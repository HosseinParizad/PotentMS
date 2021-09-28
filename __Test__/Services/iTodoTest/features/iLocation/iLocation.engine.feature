Feature: iLocation
	Location main engine 

Scenario: User move to location
	Given Register location service for user 'Ali'
	And Simulate location service detect 'Ali' move to 'Home'
	Then I get feedback 'Member move to new location' with content '{"MemberKey":"Ali","NewLocation":"Home"}'

Scenario: Order by location
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | MemberKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      | Ali       |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      | Ali       |          |
	When User add location 'Home' to task '1D7335DA-7013-482A-A23F-62CB24939EE6' for member 'Ali' in group 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text                                | GroupKey | MemberKey | ParentId | Info            |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      | Ali       |          |                 |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      | Ali       |          | Locations: Home |
	Then Asstant should ask me to do following tasks:
		| Id                                   | Text                                | GroupKey | MemberKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      | Ali       |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      | Ali       |          |
	Given Simulate location service detect 'Ali' move to 'Home'
	Then I get feedback 'Member move to new location' with content '{"MemberKey":"Ali","NewLocation":"Home"}'
	Then Asstant should ask me to do following tasks:
		| Id                                   | Text                                | GroupKey | MemberKey | ParentId |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      | Ali       |          |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      | Ali       |          |