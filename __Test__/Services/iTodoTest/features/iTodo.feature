Feature: Add

	Scenario: Send a task should add task to list
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | @i       |
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | @i       |

	Scenario: User should be able to update a task
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User update description to 'Watch dog videos on YouTube all day' for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch dog videos on YouTube all day | Ali      |

	Scenario: Each person should have its own list
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | @me      |
			| Watch dog videos on YouTube all day | @you     |
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch dog videos on YouTube all day | @you     |
			| Watch cat videos on YouTube all day | @me      |


	Scenario: Should be able to set deadline
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set deadline '2021-01-07T00:00:00Z' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Deadline                  |
			| Watch cat videos on YouTube all day | Ali      | 2021-01-07T00:00:00+00:00 |

	Scenario: Should be able to send validation error
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Askhar'
		Then I should see feedback error 'what are you doing!'

