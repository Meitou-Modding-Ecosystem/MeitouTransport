"""Pair real Stoat frames by heading. Keep the original stills for full-resolution inspection."""
import argparse
import math
import re
import shutil
import subprocess
from dataclasses import dataclass
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont, ImageOps


@dataclass(frozen=True)
class Frame:
    path: Path
    tick: int
    angle: float


def turn_frames(trial):
    log = (trial / "client.log").read_text(encoding="utf-8", errors="replace")
    if "LIVE RIDE PASS" not in log:
        raise ValueError(f"Ride did not complete in {trial}")
    matches = re.findall(r"LIVE RIDE capture tick=(\d+) stage=(Run|Turn) yaw=([\d.-]+)", log)
    approach = [(int(t), float(y)) for t, stage, y in matches if stage == "Run"]
    turning = [(int(t), float(y)) for t, stage, y in matches if stage == "Turn"]
    if not approach or not turning:
        raise ValueError(f"No recorded run-up and turn in {trial}")
    start = approach[-1][1]
    frames = []
    for tick, yaw in [approach[-1], *turning]:
        path = trial / f"ride-{tick:06d}.png"
        if tick == turning[-1][0] and not path.is_file():
            # The final turn checkpoint can name the same frame turn.png.
            path = trial / "turn.png"
        if path.is_file():
            frames.append(Frame(path, tick, abs(math.degrees(math.remainder(yaw - start, math.tau)))))
    if len(frames) < 3 or frames[-1].angle < 85:
        raise ValueError(f"Missing before/middle/after turn captures in {trial}")
    return frames


def nearest(frames, angle):
    return min(frames, key=lambda f: abs(angle - f.angle))


def panel(pair, labels, title_font, detail_font):
    image = Image.new("RGB", (1440, 688), "#172125")
    draw = ImageDraw.Draw(image)
    for i, (frame, label) in enumerate(zip(pair, labels)):
        with Image.open(frame.path) as photo:
            # Identical central crops preserve the viewing angle and make the saddle visible.
            view = ImageOps.fit(photo, (720, 624), Image.Resampling.LANCZOS, bleed=.04)
        x = 720 * i
        image.paste(view, (x, 64))
        draw.text((x + 15, 6), label, fill="white", font=title_font)
        draw.text((x + 16, 39), f"Game tick {frame.tick}   |   Heading change {frame.angle:.1f} deg",
                  fill="#d9dfd9", font=detail_font)
    return image


def compose(garru, beast, output, ffmpeg):
    output.mkdir(parents=True, exist_ok=True)
    sequences = [turn_frames(garru), turn_frames(beast)]
    labels = ["Garru", "Pack Beast with stock bags"]
    title = ImageFont.load_default(size=23)
    detail = ImageFont.load_default(size=16)
    pairings = []
    for first in sequences[0]:
        pairings.append([first, nearest(sequences[1], first.angle)])
    for label, target in [("before", 0), ("side", 45), ("after", 90)]:
        pair = [nearest(seq, target) for seq in sequences]
        panel(pair, labels, title, detail).save(output / f"comparison-{label}.png")
        for animal, frame in zip(("garru", "pack-beast"), pair):
            shutil.copyfile(frame.path, output / f"{animal}-{label}.png")
    command = [ffmpeg, "-loglevel", "error", "-y", "-f", "rawvideo", "-pixel_format", "rgb24",
               "-video_size", "1440x688", "-framerate", "12", "-i", "-", "-an", "-c:v", "libx264",
               "-pix_fmt", "yuv420p", "-crf", "17", "-movflags", "+faststart", str(output / "stoat-turn.mp4")]
    with subprocess.Popen(command, stdin=subprocess.PIPE) as video:
        for i, pair in enumerate(pairings):
            pixels = panel(pair, labels, title, detail).tobytes()
            copies = 9 if i in (0, len(pairings) - 1) else 1
            for _ in range(copies):
                video.stdin.write(pixels)
        video.stdin.close()
        if video.wait():
            raise RuntimeError("Video encoder failed")
    (output / "frame-pairs.tsv").write_text("garru_tick\tgarru_degrees\tpack_beast_tick\tpack_beast_degrees\n" +
        "".join(f"{a.tick}\t{a.angle:.3f}\t{b.tick}\t{b.angle:.3f}\n" for a, b in pairings), encoding="utf-8")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    for arg in ("garru", "beast", "output"):
        parser.add_argument(arg, type=Path)
    parser.add_argument("--ffmpeg", default="ffmpeg")
    args = parser.parse_args()
    compose(args.garru, args.beast, args.output, args.ffmpeg)
