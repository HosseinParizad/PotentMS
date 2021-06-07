Feature: PersonalAssistant

	Scenario: Default Borad
		Then I should see the following board for 'Anyone':
			| Text          |
			| Goal          |
			| Tag           |
			| UsedLocations |
	Scenario: When we addd task Borad should catch the event
		Then I should see the following board for 'Ali':
			| Text          |
			| Goal          |
			| Tag           |
			| UsedLocations |
		Given I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		Then I should see the following board for 'Ali':
			| Text          |
			| Goal          |
			| Tag           |
			| UsedLocations |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following board for 'Ali':
			| Text          | Badges        |
			| Goal          | ["Deadlines"] |
			| Tag           | ["Home"]      |
			| UsedLocations | []            |
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following board for 'Ali':
			| Text          | Badges        |
			| Goal          | ["Deadlines"] |
			| Tag           | ["Home"]      |
			| UsedLocations | []            |
		When User add 'Work' to tag 0 on selected task for 'Ali'
		Then I should see the following board for 'Ali':
			| Text          | Badges          |
			| Goal          | ["Deadlines"]   |
			| Tag           | ["Home","Work"] |
			| UsedLocations | []              |
		When User add 'One,One' to tag 0 on selected task for 'Ali'
		Then I should see the following board for 'Ali':
			| Text          | Badges                |
			| Goal          | ["Deadlines"]         |
			| Tag           | ["Home","Work","One"] |
			| UsedLocations | []                    |

	# Scenario: When use see deadline show see task be deadline order
	# Then I should see the following board for 'Ali':
	# 	| Text          | Badges        |
	# 	| Goal          | ["Deadlines"] |
	# 	| Tag           | []            |
	# 	| UsedLocations | []            |
	# Given I send the following task:
	# 	| TaskDesc                            | GroupKey |
	# 	| Watch cat videos on YouTube all day | Ali      |
	# 	| Watch dog videos on YouTube all day | Ali      |
	# When User select item 1 from tasks of 'Ali'
	# When User set deadline '2021-10-07T00:00:00Z' on selected task for 'Ali'
	# When User select item 2 from tasks of 'Ali'
	# When User set deadline '2021-10-06T00:00:00Z' on selected task for 'Ali'
	# Then I should see the following todo list:
	# 	| TaskDesc                            | GroupKey | Deadline                  |
	# 	| Watch cat videos on YouTube all day | Ali      | 2021-10-07T00:00:00+00:00 |
	# 	| Watch dog videos on YouTube all day | Ali      | 2021-10-06T00:00:00+00:00 |
	# Then I should see the following board deallines:
	# 	| GroupKey | Text                                | Deadline                  |
	# 	| Ali      | Watch dog videos on YouTube all day | 2021-10-06T00:00:00+00:00 |
	# 	| Ali      | Watch cat videos on YouTube all day | 2021-10-07T00:00:00+00:00 |
	# Given I send the following task:
	# 	| TaskDesc                               | GroupKey |
	# 	| Watch donkey videos on YouTube all day | Ali      |
	# When User select item 3 from tasks of 'Ali'
	# When User set deadline '2021-10-01T00:00:00Z' on selected task for 'Ali'
	# Then I should see the following board deallines:
	# 	| GroupKey | Text                                   | Deadline                  |
	# 	| Ali      | Watch donkey videos on YouTube all day | 2021-10-01T00:00:00+00:00 |
	# 	| Ali      | Watch dog videos on YouTube all day    | 2021-10-06T00:00:00+00:00 |
	# 	| Ali      | Watch cat videos on YouTube all day    | 2021-10-07T00:00:00+00:00 |

	Scenario: User should be able to see tidy board
		Given I send the following task:
			| TaskDesc                  | GroupKey |
			| Do Project A              | Me       |
			| Finish task on Pave board | Me       |
			| Buy coca                  | Me       |
			| Buy drink                 | Me       |
		When User select item 1 from tasks of 'Me'
		When User set location 'Home -> My room' on selected task for 'Me'
		When User add 'Very special tag' to tag 0 on selected task for 'Me'
		When User select item 2 from tasks of 'Me'
		When User set location 'Work -> Office' on selected task for 'Me'
		When User select item 3 from tasks of 'Me'
		When User set location 'Shop -> Woolies' on selected task for 'Me'
		When User select item 4 from tasks of 'Me'
		When User set location 'Shop -> Liquor' on selected task for 'Me'

		Then I should see the following board for 'Me':
			| Text          | Badges                                                                  |
			| Goal          | ["Deadlines"]                                                           |
			| Tag           | ["Very special tag"]                                                    |
			| UsedLocations | ["Home -> My room","Work -> Office","Shop -> Woolies","Shop -> Liquor"] |
