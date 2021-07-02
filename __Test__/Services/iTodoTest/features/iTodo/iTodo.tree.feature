Feature: Tree

	Scenario: Should be able to make a sub task
		Given  I send the following task:
			| TaskDesc | GroupKey |
			| My goal  | Ali      |
		When User select item 1 from tasks of 'Ali'
		Given  I send the following task:
			| TaskDesc | GroupKey |
			| My step  | Ali      |
		Then I should see the following todo list:
			| TaskDesc | GroupKey | ParentId     |
			| My goal  | Ali      |              |
			| My step  | Ali      | [selectedid] |
