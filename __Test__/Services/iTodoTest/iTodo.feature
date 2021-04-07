Feature: Add

	Scenario: Add two numbers
		When i add 2 and 3
		Then the result should be 5

	Scenario: Add a task
		When i send a request to add task bla for Ali
		Then the result should be 5
