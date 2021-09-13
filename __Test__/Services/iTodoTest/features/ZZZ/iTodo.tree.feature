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

Scenario: Move task
	Given  I send the following task:
		| TaskDesc  | GroupKey |
		| My goal 1 | Ali      |
		| My goal 2 | Ali      |
		| My step   | Ali      |
	Then I should see the following todo list:
		| TaskDesc  | GroupKey | ParentId |
		| My goal 1 | Ali      |          |
		| My goal 2 | Ali      |          |
		| My step   | Ali      |          |
	When User select item 3 from tasks of 'Ali'
	And User select Copy
	When User select item 2 from tasks of 'Ali'
	And User select Paste to group 'Ali'
	When User select item 2 from tasks of 'Ali'
	Then I should see the following todo list:
		| TaskDesc  | GroupKey | ParentId     |
		| My goal 1 | Ali      |              |
		| My goal 2 | Ali      |              |
		| My step   | Ali      | [selectedid] |
# goal 2 is selected