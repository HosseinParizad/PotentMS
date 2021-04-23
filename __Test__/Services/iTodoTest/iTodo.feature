Feature: Add

	# Scenario: Send a task should add task to list
	# 	Given  I send the following task:
	# 		| TaskDesc                            | GroupKey |
	# 		| Watch cat videos on YouTube all day | @i       |
	# 	Then I should see the following todo list:
	# 		| TaskDesc                            | GroupKey |
	# 		| Watch cat videos on YouTube all day | @i       |

	Scenario: User should be able to update a task
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | ALi      |
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | ALi      |
		When User select item 1
		When User update description to 'Watch dog videos on YouTube all day' for 'ALi'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch dog videos on YouTube all day | ALi      |

# Scenario: Each person should have its own list
# 	Given  I send the following task:
# 		| TaskDesc                            | GroupKey |
# 		| Watch cat videos on YouTube all day | @me      |
# 		| Watch dog videos on YouTube all day | @you     |
# 	Then I should see the following todo list:
# 		| TaskDesc                            | GroupKey |
# 		| Watch cat videos on YouTube all day | @me      |
# 		| Watch dog videos on YouTube all day | @you     |

# Scenario: Task should order as enter be default
# 	Given  I send the following task:
# 		| TaskDesc                            | GroupKey |
# 		| Watch cat videos on YouTube all day | @me      |
# 		| Do what ever you want               | @you     |
# 		| Eat what ever you want              | @you     |
# 		| check the reault :)                 | @you     |
# 	Then I should see the following todo list:
# 		| TaskDesc                            | GroupKey | Index |
# 		| Watch cat videos on YouTube all day | @me      | 0     |
# 		| Do what ever you want               | @you     | 0     |
# 		| Eat what ever you want              | @you     | 1     |
# 		| check the reault :)                 | @you     | 2     |



























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
