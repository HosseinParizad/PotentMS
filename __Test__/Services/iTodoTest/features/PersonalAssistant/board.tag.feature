Feature: PersonalAssistant.tag

	Scenario: When select tag on board ishould see all active task has this tag
		Given I send the following task:
			| TaskDesc               | GroupKey |
			| Somthing doing at home | Me       |
			| Somthing to do at work | Me       |
		When User select item 1 from tasks of 'Me'
		When User add 'Home' to tag 0 on selected task for 'Me'
		When User select item 2 from tasks of 'Me'
		When User add 'Work' to tag 0 on selected task for 'Me'
		Then I should see the following board:
			| AssistantKey | Text | Badges          |
			| Me           | Goal | ["Deadlines"]   |
			| Me           | Tag  | ["Home","Work"] |
		Then I should see the following tasks for selected tag 'Home' for 'Me':
			| Text                   |
			| Somthing doing at home |
		Given I send the following task:
			| TaskDesc           | GroupKey |
			| do nothing at home | You      |
		When User select item 1 from tasks of 'You'
		When User add 'Home' to tag 0 on selected task for 'You'
		Then I should see the following tasks for selected tag 'Home' for 'Me':
			| Text                   |
			| Somthing doing at home |
		Then I should see the following tasks for selected tag 'Home' for 'You':
			| Text               |
			| do nothing at home |
		Then I should see the following tasks for selected tag 'Work' for 'Me':
			| Text                   |
			| Somthing to do at work |


	Scenario: Should be able see tag by location
		Given  I send the following task:
			| TaskDesc  | GroupKey |
			| Buy carot | Me       |
			| Eat carot | Me       |
		When User select item 1 from tasks of 'Me'
		When User set location 'Shop -> Woolies' on selected task for 'Me'
		When User select item 2 from tasks of 'Me'
		When User set location 'Home -> dining room -> table' on selected task for 'Me'
		Then I should see the following todo list:
			| TaskDesc  | GroupKey | Locations                        |
			| Buy carot | Me       | ["Shop -> Woolies"]              |
			| Eat carot | Me       | ["Home -> dining room -> table"] |
		Then I should see the following tasks when 'Me' go to 'Shop':
			| Text      |
			| Buy carot |
		Then I should see the following tasks when 'Me' go to 'Home':
			| Text      |
			| Eat carot |
