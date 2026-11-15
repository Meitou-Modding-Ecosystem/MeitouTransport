# Stoat turn with a native saddle mesh

Meitou placed the rider with `DrawAnchor` and drew the saddle with `CharacterCold.SetBoneMesh` on both animals. The run used an unmodified Windows x64 source build of Meitou commit `b3cb76581108fb8d9d6ea551c8a08db4d26c2488`, labeled locally `0.19.1-local.740`. The packaged nightly from the earlier rider trial predates this API. [Martin’s answer](https://github.com/VlcekM/MeitouClient/issues/740#issuecomment-6888417582) · [Upstream implementation](https://github.com/VlcekM/MeitouClient/commit/b3cb76581108fb8d9d6ea551c8a08db4d26c2488)

| Animal | Posed frames | Frames in turn | Heading change | Max saddle-origin error | Max rider-pelvis error |
| --- | ---: | ---: | ---: | ---: | ---: |
| Garru | 119 | 41 | 91.8° | 0.00873 | 0.01747 |
| Pack Beast with stock bags | 119 | 41 | 92.0° | 0.00873 | 0.01611 |

The verifier read the keyed mesh from the drawn snapshot, checked that Meitou submitted exactly one matching saddle with a native draw ID, and measured its authored origin and the rider’s posed pelvis independently. The limit was 0.08 world units per frame. Both animals retained the expected facing and scale. The maximum saddle error matched the previous private renderer hook. At broadside (45.2° and 45.5°), the Pack Beast’s upper bag still overlaps the rear of the saddle in silhouette. [Earlier Stoat measurements](stoat-draw-anchor.md)

Both profiles loaded Transport, the same Maestro-exported Garru Saddle ZIP (SHA-256 `59139598506042d1d9645cf0e98d3c7da8a6e4c2c4740ba516d66c5fa5cd23f7`), and the Stoat start. The Pack Beast profile substituted its stock animal record in the start. Both rides passed saving while mounted, reloading on foot, remounting, dismounting and walking on an ordinary order. The Pack Beast showed its stock skinned bags in all 119 measured frames. After five frames out of view, both characters returned together with one saddle for eight consecutive aligned frames. The equipped backpack and all three inventory entries survived.

The first Garru attempt reached the verifier’s distance checkpoint while still turning at 38.5°. The final runs used the same destinations and camera, and waited for the animal’s heading to reach the commanded turn. The existing acceptance limits for angle, position, facing, and scale remained in effect.

Transport sets the namespaced saddle key once at mount and removes it on release. Its pack format and art stay the same. A focused regression test placed a second mod’s mesh on the same carrier, exercised the saddle on a rotated bind bone at scales 0.55 and 1.45, and verified that removing the saddle preserved the other mesh. One and four workers matched across 333 voyage ticks and 216 riding ticks. [Local reproduction](../tests/LiveVoyage/README.md#reproduce-the-stoat-turn)
