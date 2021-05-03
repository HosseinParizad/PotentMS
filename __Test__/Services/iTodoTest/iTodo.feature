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

	Scenario: Should be able to send vAlidation error
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set tag 'Home' on selected task for 'Askhar'
		Then I should see feedback error 'what are you doing!'

























# Scenario: Should be able to set deadline for a task
# 	When I send the following task:
# 		| Id  | GroupId | TaskDesc                            |
# 		| 123 | @me     | Watch cat videos on YouTube all day |
# 		| 125 | @me     | Watch dog videos on YouTube all day |
# 	And I send deadline '12/12/2021 12:34' for task id '123'
# 	Then I should see the following todo list:
# 		| Id  | TaskDesc                            | GroupKey | Deadline        | Index |
# 		| 123 | Watch cat videos on YouTube all day | @me     | 12/12/2021 12:34 | 0     |
# 		| 125 | Watch dog videos on YouTube all day | @me     |                  | 1     |

# Scenario: Should not be able to set deadline in past
# 	When I send the following task:
# 		| Id  | GroupId | TaskDesc                            |
# 		| 123 | @me     | Watch cat videos on YouTube all day |
# 	And I send deadline '12/12/2010 12:34' for task id '123'
# 	Then I should see the following todo list:
# 		| Id  | TaskDesc                            | GroupKey| Deadline | Index |
# 		| 123 | Watch cat videos on YouTube all day | @me     |          | 0     |
# 		| 125 | Watch dog videos on YouTube all day | @me     |          | 1     |
