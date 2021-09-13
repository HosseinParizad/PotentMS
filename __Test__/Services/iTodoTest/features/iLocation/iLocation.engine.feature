Feature: iLocation
	Location main engine 

Scenario: Set location for task
	Given User have a selected service
	When User add location 'Shop > Woolies' to selected task
	Then I get feedback 'Task has been started'
		| Group   |
		| Hossein |
	Then I should see the following Group list:
		| Text    | GroupKey |
		| Hossein | Hossein  |