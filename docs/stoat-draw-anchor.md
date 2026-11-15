# Stoat rider-anchor trial

Transport `0.3.0-qa.602` passed the desert turn on an adult Garru and an adult Pack Beast wearing its stock bags. In both drawn snapshots Meitou placed Sable on the carrier's `Bip01 Spine1`. Transport submitted one separate saddle. Both rides used the same Maestro-exported schema-3 fit, route, fixed camera angle and Scorchlander outfit.

## Attachment measurements

Maximum position errors are in world units. Each frame had to stay below **0.08** for both the loose saddle's authored origin and the rendered rider's pelvis. “Posed” counts frames from mounting through the end of the turn. The turn frames are a subset. The v0.18.0 comparison uses the earlier Maestro export trials.

| Client and animal | Posed | Turn frames | Heading change | Max saddle error | Max pelvis error |
| --- | ---: | ---: | ---: | ---: | ---: |
| v0.18.0 Garru | 148 | 33 | 91.9° | 0.00873 | 0.00873 |
| Nightly Garru | 145 | 30 | 91.8° | 0.00873 | 0.01611 |
| v0.18.0 Pack Beast | 136 | 31 | 91.9° | 0.00873 | 0.00873 |
| Nightly Pack Beast | 142 | 30 | 91.8° | 0.00873 | 0.02376 |

The original v0.18.0 public-pack runs also peaked at 0.00873 for both measurements. They recorded 149 posed frames on the Garru and 148 on the Pack Beast, with 34 turn frames and a 91.8° turn on each.

The nightly verifier compared the submitted mesh with the carrier's actual drawn bone and independently compared the rider's posed pelvis with the seat computed from that bone. Saddle rotation and rider facing each passed a dot-product threshold of 0.999. Saddle scale error stayed under 0.005 and rider scale error under 0.01. Each ride covered idle, walk, run and 11.4 units of elevation change. Checks during saving, remounting and the Pack Beast's return to view kept the same maxima.

## What the camera shows

The side-by-side clip pairs 31 actual captures by heading and plays them at 12 fps with a short hold at either end. The paired stills show 0.0°, 44.3° and 89.9° on both animals. The full-resolution stills in the kit preserve the original 1280 × 800 frames.

At 44.3° the Pack Beast's upper bundle overlaps the rear edge of the saddle in silhouette. The saddle panel also hides part of the near shin on the bare Garru. At 89.9° the feet are visible forward of the upper bundle. The stills show which surfaces obscure one another from this camera angle. Clearance between the underlying meshes remains unmeasured.

## Save, camera travel and input

- Both saves completed mid-ride with the original animal still selected and ridden. After reload, the original squad members were on foot 11.0 units apart. Sable remounted and dismounted, then walked 20.2 units on an ordinary order. Each animal stopped. The additional Domesticated Bull ride also passed.
- The Pack Beast displayed its native skinned luggage on all 142 posed frames. With the camera moved away, both character world entries and the saddle were absent for five consecutive frames. On return both characters reappeared together, with one matching saddle for eight consecutive aligned frames. The equipped backpack (`4002-gamedata.base`) and all three inventory entries survived. Neither actor appeared alone during the transition.
- One and four simulation workers produced identical per-tick hashes for 333 voyage ticks and 216 riding ticks. A separate transform check used a rotated bind bone and carrier scales 0.55 and 1.45. The seat moved with each carrier size and the rider kept the same proportions. The two live adults each had scale 1.

## Build and inputs

The candidate is based on Transport `v0.3.0-playtest.4` (`d4ed8f3`). The Windows x64 game, build references and verifier used `v0.19.1-nightly.20261114.3`, commit `fb8282dc652ac2975e24c2d68e55dc60cf7f7b5a`. The mod manifest matches numeric game version `=0.19.1`. The runner additionally checked the executable's full product version. The official game ZIP matched GitHub’s SHA-256, `e546ddb9e613e2add386dd7dc1c04cde37bbafd5b6438293f23029245722ed60`. [Pinned game release](https://github.com/VlcekM/MeitouClient/releases/tag/v0.19.1-nightly.20261114.3) · [Native anchor contract](https://github.com/VlcekM/MeitouClient/blob/v0.19.1-nightly.20261114.3/docs/character-renderer.md#bone-anchors-meitou-engine-choice)

Each fresh profile loaded **Meitou Transport → Garru Saddle → Stoat Desert Ride**. The Maestro ZIP had SHA-256 `59139598506042d1d9645cf0e98d3c7da8a6e4c2c4740ba516d66c5fa5cd23f7`. The verifier checked every installed file against it. The Pack Beast trial used a copy of the Stoat start with the stock Pack Beast in the Garru slot. The only code mod in either profile was Transport.
