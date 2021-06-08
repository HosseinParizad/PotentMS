Feature: PersonalAssistant.Group

	Scenario: When select group should get board by member
		Given Send an email '@family' to create group
		Given Add '@me' as member of '@family'
		Given Add '@you' as member of '@family'
		Then I should see the following groups:
			| Group   | Member  |
			| @family | @family |
			| @family | @me     |
			| @family | @you    |
			| @me     | @me     |
			| @you    | @you    |

		Then I should see the following board for '@family':
			| Text    | Badges |
			| @family | []     |
			| @me     | []     |
			| @you    | []     |
