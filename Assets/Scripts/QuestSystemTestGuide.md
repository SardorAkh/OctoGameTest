# Quest System Test Guide

## Overview
Your quest system is now ready for testing! It includes:
- **Quest Management**: Start, track, and complete quests
- **Task Dependencies**: Tasks can require other tasks to be completed first
- **Persistence**: Quest progress is saved using Naninovel's variable system
- **Integration**: Works with minigames and other game systems

## Test Scripts Available

### 1. `Test.nani` - Basic Quest System Test
This script tests:
- Starting quests
- Completing tasks
- Checking quest status
- Task dependencies
- Quest persistence

### 2. `QuestMinigameTest.nani` - Integration Test
This script tests:
- Quest + Minigame integration
- Conditional quest progression based on minigame results
- Multiple minigame attempts

## How to Test

### Step 1: Run the Basic Test
1. Open Unity
2. Go to `Assets/Scenario/Test.nani`
3. Play the scene
4. Follow the script - it will automatically test all quest features

### Step 2: Run the Integration Test
1. Open `Assets/Scenario/QuestMinigameTest.nani`
2. This will test quest + minigame integration
3. You'll need to complete the pairs minigame to progress

## Quest Configuration

The quests are configured in `Assets/NaninovelData/Resources/Naninovel/Configuration/QuestConfiguration.asset`:

### Available Quests:
1. **tutorial_quest**: Simple 3-task quest
   - Talk to NPC
   - Play minigame (requires talking to NPC)
   - Visit location (requires completing minigame)

2. **advanced_quest**: 2-task quest
   - Collect item
   - Talk to Chinatsu (requires collecting item)

3. **main_story**: Original quest
   - Talk to Chinatsu
   - Visit Kohaku (requires talking to Chinatsu)

## Available Commands

### In Naninovel Scripts:
```nani
@startQuest questId:"quest_name"
@completeTask "quest_name" task:"task_id"
```

### Expression Functions:
```nani
@if IsQuestActive("quest_name")
@if IsQuestCompleted("quest_name")
@if HasCompletedTask("quest_name" "task_id")
{GetCurrentTaskDescription("quest_name")}
```

## Task Types

The system supports these task types:
- **TalkTo (0)**: Talk to a character
- **CollectItem (1)**: Collect an item
- **VisitLocation (2)**: Visit a specific location
- **PlayMinigame (3)**: Complete a minigame
- **Custom (4)**: Custom task type

## Testing Checklist

- [ ] Quest starts properly
- [ ] Tasks complete correctly
- [ ] Task dependencies work (can't complete task 2 before task 1)
- [ ] Quest completion detection works
- [ ] Quest persistence works (progress saves)
- [ ] Minigame integration works
- [ ] Expression functions return correct values
- [ ] Multiple quests can be active simultaneously

## Troubleshooting

### Common Issues:
1. **Quest doesn't start**: Check if quest ID exists in configuration
2. **Task doesn't complete**: Verify task ID and quest is active
3. **Dependencies not working**: Ensure required tasks are completed first
4. **Persistence issues**: Check if variables are being saved properly

### Debug Commands:
Add these to your scripts for debugging:
```nani
@print "Quest Active: {IsQuestActive('quest_name')}"
@print "Quest Completed: {IsQuestCompleted('quest_name')}"
@print "Current Task: {GetCurrentTaskDescription('quest_name')}"
```

## Next Steps

After testing, you can:
1. Create more complex quest chains
2. Add custom task types
3. Integrate with character dialogue
4. Add quest rewards
5. Create quest UI elements

Happy testing! 🎮
