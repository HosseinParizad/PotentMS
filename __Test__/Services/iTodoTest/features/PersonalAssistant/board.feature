Feature: PersonalAssistant

	Scenario: Default Borad
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |

	Scenario: When we addd task Borad should catch the event
		Given I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |
