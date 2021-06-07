Feature: PersonalAssistant.Location

	Scenario: Should see the location section
		Then I should see the following board for 'Me':
			| Text          | Badges        |
			| Goal          | ["Deadlines"] |
			| Tag           | []            |
			| UsedLocations | []            |


	Scenario: Should get location event
		Given  I send the following task:
			| TaskDesc  | GroupKey |
			| Something | Me       |
		When User select item 1 from tasks of 'Me'
		When User 'Me' go to 'Home'
		Then I should see the following board for 'Me':
			| Text            | CurrentLocation |
			| CurrentLocation | Home            |
