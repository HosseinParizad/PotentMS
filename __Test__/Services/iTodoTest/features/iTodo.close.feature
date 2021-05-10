Feature: Close task

	Scenario: Should be able to close task
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
			| Watch dog videos on YouTube all day | Ali      |
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
			| Watch dog videos on YouTube all day | Ali      |
		When User select item 2 from tasks of 'Ali'
		When User close selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |



	Scenario: Should not be able to close task for wrong group key
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User close selected task for 'Akbar'
		Then I should see feedback error 'Cannot find task!'
