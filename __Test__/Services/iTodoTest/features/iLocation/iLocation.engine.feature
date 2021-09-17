Feature: iLocation
	Location main engine 

Scenario: User move to location
	Given Register location service for user 'Ali'
	And Simulate location service detect 'Ali' move to 'Home'
	Then I get feedback 'Member move to new location' with content '{"NewLocation":"Home"}'