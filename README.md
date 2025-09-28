<!-- GIFs showcase -->
<table width="100%">
  <tr>
    <!-- Top large gif -->
    <td colspan="2" align="center">
      <img src="https://github.com/wi1wil/LST/raw/main/vid3.gif" width="100%"/>
    </td>
  </tr>
  <tr>
    <!-- Bottom two gifs -->
    <td align="center" width="50%">
      <img src="https://github.com/wi1wil/LST/raw/main/vid1.gif" width="100%"/>
    </td>
    <td align="center" width="50%">
      <img src="https://github.com/wi1wil/LST/raw/main/vid2.gif" width="100%"/>
    </td>
  </tr>
</table>

--- 

<!-- About the game -->
<table width="100%">
  <tr>
    <!-- Left Image -->
    <td width="30%" align="center" valign="middle" style="padding:15px;">
      <img src="https://github.com/wi1wil/LST/raw/main/LST2.png" width="220"/>
    </td>
    <!-- Right Text -->
    <td width="70%" valign="top" style="padding:15px;">
      <h2>🌍 About </h2>
      <p style="max-width:700px;">
        LST is an action-packed mage survival game with roguelike elements. 
        Players wield powerful abilities such as Fireball, Icicle Surge, and Dash to battle waves of enemies across procedurally generated maps. 
        Archers and other foes will hunt you down, forcing you to strategize with magic, mobility, and resource management.
      </p>
      <!-- <a href="https://wi1wil.itch.io/lst">
        <img src="https://img.shields.io/badge/Itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white" />
      </a> -->
    </td>
  </tr>
</table>

---

<h2>💡 My Contributions</h2>
<p>
This project is fully developed as a <b>solo project</b>, where I was responsible for all of the 
core mechanics and systems. These include the <b>fireball</b> and <b>icicle surge</b> abilities, 
the <b>dash system</b> for fast mobility, enemy AI such as <b>archers with attack and chase states</b>, 
and the <b>procedural map generation</b> that ensures every run feels unique. 
I also integrated the <b>inventory system</b> to manage items during gameplay, making the 
combat and progression loop more engaging.
</p>

---

<h2>📜 Key Scripts</h2>
<table>
  <tr>
    <th>Script</th>
    <th>Description</th>
  </tr>

  <tr>
    <td><code>ArcherAttackState.cs</code></td>
    <td>Controls how archers detect and attack the player, managing ranged combat logic.</td>
  </tr>
  <tr>
    <td><code>ArcherChaseState.cs</code></td>
    <td>Handles enemy AI chasing behavior, pursuing the player within detection range.</td>
  </tr>
  <tr>
    <td><code>MapGenerator.cs</code></td>
    <td>Implements procedural map generation for dynamic levels and replayability.</td>
  </tr>
  <tr>
    <td><code>InventoryManagerScript.cs</code></td>
    <td>Manages collected items, potions, and resources during gameplay.</td>
  </tr>
  <tr>
    <td><code>IcicleSurgeScript.cs</code></td>
    <td>Defines the mage’s Icicle Surge spell behavior, including damage and visual effects.</td>
  </tr>
</table>

---

<pre>
LST                               # Root folder of the project
└── Assets                        # Unity assets, scripts, and scenes
    ├── _Animation                # Stores animation clips and controllers
    ├── _Audio                    # Background music and sound effects
    ├── _Prefabs                  # Pre-configured reusable game objects
    ├── _Scenes                   # All Unity Scenes (menus, levels, etc.)
    ├── _Scripts                  # Parent folder of all C# scripts
    │   ├── Attacks & Abilities   # Player spells (Fireball, Dash, Icicle Surge, etc.)
    │   ├── Enemy                 # AI behavior (Archer, Berserker, etc.)
    │   ├── Inventory             # Inventory system management
    │   ├── Map                   # Procedural map generation
    │   └── Player                # Player health, movement, and interactions
    ├── _Sprites                  # 2D sprites and UI assets
    └── _Tilesets                 # Tilemaps for level generation
</pre>

---

<h2>🎮 Controls / Inputs</h2>

<table>
  <tr>
    <th>Action</th>
    <th>Key / Input</th>
  </tr>
  <tr>
    <td>Move Left</td>
    <td><b>A</b> / <b>←</b></td>
  </tr>
  <tr>
    <td>Move Right</td>
    <td><b>D</b> / <b>→</b></td>
  </tr>
  <tr>
    <td>Move Up</td>
    <td><b>W</b> / <b>↑</b></td>
  </tr>
  <tr>
    <td>Move Down</td>
    <td><b>S</b> / <b>↓</b></td>
  </tr>
  <tr>
    <td>Cast Fireball</td>
    <td><b>F</b></td>
  </tr>
  <tr>
    <td>Cast Icicle Surge</td>
    <td><b>Q</b></td>
  </tr>
  <tr>
    <td>Dash</td>
    <td><b>V</b></td>
  </tr>
  <tr>
    <td>Open Inventory </td>
    <td><b>Tab</b></td>
  </tr>
</table>

---

<!-- Credits -->
  <tr>
    <td align="left" style="padding:20px;">
      <h2>🎵 Assets & Credits</h2>
      <p>All assets used in this project are free and sourced from <b>itch.io</b>.</p>
      <a href="https://itch.io/game-assets/free">
        <img src="https://img.shields.io/badge/Assets-itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white" />
      </a>
    </td>
  </tr>

---
