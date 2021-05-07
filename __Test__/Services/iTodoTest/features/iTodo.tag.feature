Feature: Tag

	Scenario: Adding Tag
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set tag 'Home' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags     |
			| Watch cat videos on YouTube all day | Ali      | ["Home"] |

	Scenario: Should be able to set tag
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set tag 'Home' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags     |
			| Watch cat videos on YouTube all day | Ali      | ["Home"] |
		When User set tag 'Garden' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags              |
			| Watch cat videos on YouTube all day | Ali      | ["Home","Garden"] |
