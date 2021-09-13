Feature: PersonalAssistant.Memory

Scenario: When user add memory task then should see in
	Given Send an email 'family' to create group
	Given Add 'Hossein' as member of 'family'
	Given  I send the following memory:
		| TaskDesc        | GroupKey |
		| Remember stupid | Hossein  |
	Then I should see the following memory list:
		| TaskDesc        | GroupKey |
		| Remember stupid | Hossein  |
	Then I should see the following board for 'Hossein':
		| Text          |
		| Goal          |
		| Tag           |
		| UsedLocations |
		| Due           |
		| Task          |
		| Ordered       |
		| Memorizes     |