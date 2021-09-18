Feature: iAssistant
	Assistant main engine 

Scenario: See task need to do
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |
	Then Asstant should ask me to do following tasks:
		| Id                                   | Text                                | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |

Scenario: Order by location
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |
	When User add location 'Home' to task '1D7335DA-7013-482A-A23F-62CB24939EE6' for group 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text                                | GroupKey | ParentId | Info            |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |                 |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          | Locations: Home |
	Then Asstant should ask me to do following tasks:
		| Id                                   | Text                                | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |
	Given Simulate location service detect 'Ali' move to 'Home'
	Then I get feedback 'Member move to new location' with content '{"NewLocation":"Home"}'
	Then Asstant should ask me to do following tasks:
		| Id                                   | Text                                | GroupKey | ParentId |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |