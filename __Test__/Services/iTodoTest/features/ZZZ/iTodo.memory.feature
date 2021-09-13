Feature: Memory

Scenario: Should be able to make a memory task
	Given  I send the following memory:
		| TaskDesc        | GroupKey |
		| Remember stupid | me       |
	Then I should see the following memory list:
		| TaskDesc        | GroupKey |
		| Remember stupid | me       |

Scenario: Should be able to delete a memory
	Given  I send the following memory:
		| TaskDesc         | GroupKey |
		| Remember stupid  | me       |
		| Remember stupid2 | me       |
	Then I should see the following memory list:
		| TaskDesc         | GroupKey |
		| Remember stupid  | me       |
		| Remember stupid2 | me       |
	When User delete memory item 1 from 'me'
	Then I should see the following memory list:
		| TaskDesc         | GroupKey |
		| Remember stupid2 | me       |