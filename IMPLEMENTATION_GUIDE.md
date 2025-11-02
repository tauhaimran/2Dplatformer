# Level Progression & Coin System Implementation Guide

## What's Been Implemented

### 1. Level Progression System
- When a player completes a level (reaches the exit), they automatically progress to the next level
- Level sequence: level 1 → level 2 → level 3 → level 4 → level 5 → MainMenu
- Completing a level unlocks the next level in the main menu

### 2. Coin Persistence
- All coins collected during gameplay are saved to the player's total
- Total coins persist across all levels and game sessions
- Coins are displayed in the main menu

### 3. Level Locking System
- Players must complete level 1 to unlock level 2, and so on
- Locked levels show "locked" text and have disabled buttons
- Progress is saved using PlayerPrefs

## Setup Instructions

### Step 1: Main Menu Setup
1. Open the MainMenu scene
2. Select the menu GameObject that has the `menu.cs` script
3. In the Inspector, find the new "COIN DISPLAY" section
4. Create a TextMeshPro text object in your UI (or use existing one)
5. Drag that text object into the "Total Coins Text" field
6. This will display: "Total Coins: X"

### Step 2: PlayerManager Setup
1. Create an empty GameObject in your MainMenu scene
2. Name it "PlayerManager"
3. Add the `playerManager.cs` script to it
4. The script is now set to DontDestroyOnLoad, so it persists across scenes

### Step 3: Level Button Setup (Already Done)
Your level buttons should already be set up in the menu script with:
- levelButtons array (all 5 level buttons)
- levelLockTexts array (text showing when locked)
- The script now automatically loads unlocked levels from PlayerPrefs

### Step 4: Build Settings
Make sure all scenes are added to Build Settings in this order:
1. MainMenu
2. level 1
3. level 2
4. level 3
5. level 4
6. level 5

To add scenes:
- File → Build Settings
- Drag all scene files from Assets/Scenes into the "Scenes In Build" list

### Step 5: Level Exit Setup (For Each Level)
In each level scene (level 1 through level 5):
1. Make sure your exit door/portal has:
   - A Collider2D with "Is Trigger" checked
   - Tag set to "exit"
2. The player will automatically progress to the next level when touching it

### Step 6: Coin Setup (Already Done)
Your coins should already have:
- Tag: "coin"
- Collider2D with "Is Trigger" checked
- The playerMovement script now automatically saves coins to playerManager

## How It Works

### Level Progression Flow
1. Player completes level 1 (reaches exit)
2. System checks current level number
3. If this is the first time completing this level, it unlocks the next level
4. Player is automatically loaded into level 2
5. Progress is saved to PlayerPrefs

### Coin System Flow
1. Player collects a coin in any level
2. Coin is added to local counter (for level completion check)
3. Coin is also added to playerManager.coins (persistent total)
4. Total is saved to PlayerPrefs immediately
5. Main menu displays the total coins

### Level Locking Flow
1. Main menu loads
2. menu.cs reads unlockedLevels from playerManager
3. playerManager loads data from PlayerPrefs
4. Buttons are enabled/disabled based on unlocked levels
5. Lock text is shown/hidden accordingly

## Testing

### Test Level Progression
1. Start a new game (or reset data)
2. Play through level 1 and reach the exit
3. You should automatically load into level 2
4. Return to main menu - level 2 button should now be unlocked

### Test Coin Persistence
1. Collect some coins in level 1
2. Return to main menu (or complete the level)
3. Check that "Total Coins: X" shows the correct amount
4. Play another level and collect more coins
5. Return to menu - total should be cumulative

### Reset Progress (For Testing)
Add this code to a button or call it from console:
```csharp
playerManager.Instance.ResetData();
```

## Optional: Add Level Select Buttons

If you want to add onClick events to your level buttons:
1. Select each level button in the MainMenu scene
2. In the Inspector, find the Button component
3. Add onClick event
4. Drag the LevelLoader script (create an empty GameObject with this script)
5. Select function: LevelLoader → LoadLevel(int)
6. Set the parameter to the level number (1, 2, 3, 4, or 5)

## Files Modified
- `Assets/scripts/playerMovement.cs` - Added coin persistence and level progression
- `Assets/scripts/playerManager.cs` - Enabled DontDestroyOnLoad
- `Assets/scripts/menu.cs` - Added coin display and data loading
- `Assets/scripts/LevelLoader.cs` - NEW: Helper script for loading levels

## Troubleshooting

**Levels not unlocking:**
- Make sure playerManager GameObject exists in MainMenu scene
- Check that DontDestroyOnLoad is enabled in playerManager.cs
- Verify scene names match exactly: "level 1", "level 2", etc.

**Coins not saving:**
- Ensure playerManager.Instance is not null
- Check that coins have the "coin" tag
- Verify PlayerPrefs is working (check PlayerPrefs path)

**Wrong level loading:**
- Check Build Settings - all scenes must be added
- Verify scene names are exact: "level 1" not "Level 1"
- Check GetNextLevelName() function in playerMovement.cs

**Level buttons not working:**
- Ensure levelButtons array is populated in menu.cs
- Check that buttons have the Button component
- Verify LevelLoader script is attached to a GameObject
