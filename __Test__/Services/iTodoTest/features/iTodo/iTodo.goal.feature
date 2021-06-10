Feature: Goal

	Scenario: Adding goal
		Given Send an email '@me' to create group
		Given I send the following goals:
			| Goal           | GroupKey |
			| Finish project | @me      |
		Then I should see the following todo list:
			| TaskDesc       | GroupKey |
			| Finish project | @me      |

	Scenario: Adding goal for members
		Given Send an email '@family' to create group
		Given Add '@Hossein' as member of '@family'
		Given Add '@Mania' as member of '@family'
		Given Add '@Yasmin' as member of '@family'
		Given I send the following goals:
			| Goal                          | GroupKey |
			| Finish project                | @Hossein |
			| Finish mania's tasks          | @Mania   |
			| Learning                      | @Yasmin  |
			| Somthing everyone can do      | @family  |
			| Somthing else everyone can do | @family  |
