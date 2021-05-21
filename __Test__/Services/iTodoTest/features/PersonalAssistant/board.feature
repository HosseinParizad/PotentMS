Feature: PersonalAssistant

	Scenario: Default Borad
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |

	Scenario: When we addd task Borad should catch the event
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |
		Given I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		Then I should see the following board:
			| AssistantKey | Text |
			| Ali          | Goal |
			| Ali          | Tag  |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following board:
			| AssistantKey | Text | Badges   |
			| Ali          | Goal | []       |
			| Ali          | Tag  | ["Home"] |
