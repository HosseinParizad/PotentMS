Feature: Memory

Scenario: Should be able to make a memory task
	Given  I send the following memory:
		| TaskDesc        | GroupKey |
		| Remember stupid | me       |
	Then I should see the following todo list:
		| TaskDesc        | GroupKey |
		| Remember stupid | me       |