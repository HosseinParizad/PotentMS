Feature: Sort

	Scenario: Default sort is based on entring data
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | @me      |
			| Watch cat videos on YouTube all day | @you     |
			| Watch dog videos on YouTube all day | @me      |
			| Watch dog videos on YouTube all day | @you     |
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | @me      |
			| Watch dog videos on YouTube all day | @me      |
			| Watch cat videos on YouTube all day | @you     |
			| Watch dog videos on YouTube all day | @you     |

# Scenario: Should be able to sort by tag
# 	Given  I send the following task:
# 		| TaskDesc                            | GroupKey |
# 		| Watch cat videos on YouTube all day | Ali      |
# 		| Watch dog videos on YouTube all day | Ali      |
# 	When User select item 1 from tasks of 'Ali'
# 	When User set tag 'Home' on selected task for 'Ali'
# 	When User select item 2 from tasks of 'Ali'
# 	When User set tag 'Work' on selected task for 'Ali'
# 	Then I should see the following todo list:
# 		| TaskDesc                            | GroupKey | Tags     |
# 		| Watch cat videos on YouTube all day | Ali      | ["Home"] |
# 		| Watch dog videos on YouTube all day | Ali      | ["Work"] |
# 	When User 'Ali' go to 'Work'
# 	Then I should see the following todo list:
# 		| TaskDesc                            | GroupKey | Tags     |
# 		| Watch dog videos on YouTube all day | Ali      | ["Work"] |
# 		| Watch cat videos on YouTube all day | Ali      | ["Home"] |
# 	When User 'Ali' go to 'Home'
# 	Then I should see the following todo list:
# 		| TaskDesc                            | GroupKey | Tags     |
# 		| Watch cat videos on YouTube all day | Ali      | ["Home"] |
# 		| Watch dog videos on YouTube all day | Ali      | ["Work"] |
