Feature: Group

	Scenario: User na be member of a group
		Given  Send an email '@me' to create group
		Given  Send an email '@family' to create group
		Given  Add '@me' as member of '@family'
		Then I should see the following groups:
			| Group   | Member  |
			| @me     | @me     |
			| @family | @family |
			| @family | @me     |
