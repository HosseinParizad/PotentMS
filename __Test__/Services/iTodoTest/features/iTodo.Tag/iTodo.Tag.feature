Feature: iLocation
	Location main engine 

Scenario: Set location for task
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |
	When User add tag '0' 'Shop > Woolies' to task '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for group 'Ali'
	Then I get a feedback 'New tag has been added' and content contain '"Shop > Woolies"'
	Then I should see the following todo list directly:
		| Id                                   | Text                                | GroupKey | ParentId | Info                   |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          | Taks: 0-Shop > Woolies |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |                        |
	When User add tag '1' 'Home' to task '1D7335DA-7013-482A-A23F-62CB24939EE6' for group 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text                                | GroupKey | ParentId | Info                   |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          | Taks: 0-Shop > Woolies |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          | Taks: 1-Home           |