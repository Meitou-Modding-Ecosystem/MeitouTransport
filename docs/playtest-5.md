# Meitou v0.20.1 paired playtest

Windows x64, packaged MeitouClient `0.20.1+8c79093d075fac1f4ca8df53cf025b6155cac45f`, mod API 2. The saddle pack for these rides was exported by the Avalonia editor on Ubuntu 24.04 under WSLg. Its Linux and Windows SHA-256 was `22d26016a49fbd9cd25bf6dfc1de08493838a1c966a6b5a6651cffdfb74ab016`.

| Stoat ride | Posed frames | Turn frames | Heading change | Maximum saddle error | Maximum pelvis error |
| --- | ---: | ---: | ---: | ---: | ---: |
| Garru | 115 | 40 | 91.8° | 0.00873 | 0.01747 |
| Pack Beast with stock bags | 113 | 40 | 91.7° | 0.00873 | 0.01747 |

Errors are world units. Both stay below 0.08. Broadside, the rigid saddle sits high on the back. The stock bags partly obscure its rear edge and the rider's lower leg. The bagged turn retained the native luggage mesh in all 113 poses. After remount, five consecutive frames showed no carrier or saddle in the distant view. Returning showed one aligned saddle and the original backpack and three inventory entries for eight consecutive frames. The isolated test disabled the game's squad camera leash for that excursion.

Both rides included walking, running, saving while mounted, loading on foot, remounting, dismounting and walking away. Transport alone loaded with zero craft and actions. The skiff passed both a fresh voyage and an old-slot replay including boarding, steering and reload. The one- and four-worker simulations agreed over 333 vessel ticks and 216 riding ticks.

[Reproduce the Windows trials](../tests/LiveVoyage/README.md) · [Author the saddle](https://github.com/mattresspad/MeitouMaestro/blob/v0.1.0-playtest.1/AVALONIA.md)
