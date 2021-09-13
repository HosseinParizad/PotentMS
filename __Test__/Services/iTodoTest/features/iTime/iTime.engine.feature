Feature: iTime
	Time main engine 

Scenario: Start
	Given User have a selected service
	When User 'Start' selected task
	Then I get feedback 'Task has been started'

Scenario: Pause
	Given User have a selected service
	When User 'Start' selected task
	Then I get feedback 'Task has been started'
	When User 'Pause' selected task
	Then I get feedback 'Task has been paused'

Scenario: Done
	When User 'Done' selected task
	Then I get feedback 'Task has been done'

Scenario: Cannot pause not started task
	When User 'Pause' selected task
	Then I get feedback 'Error: cannot pause time'

Scenario: Cannot Done again
	When User 'Done' selected task
	Then I get feedback 'Task has been done'
	When User 'Done' selected task
	Then I get feedback 'Error: cannot done time'