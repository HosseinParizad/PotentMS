Feature: PersonalAssistant.Group

	Scenario: When select group should get board by member
		Given Send an email 'family' to create group
		Given Add 'Hossein' as member of 'family'
		Given Add 'Mania' as member of 'family'
		Given Add 'Yasmin' as member of 'family'

		Given I send the following goals:
			| Goal                          | GroupKey | AssignTo |
			| Finish project                | family   | Hossein  |
			| Finish mania's tasks          | family   | Mania    |
			| Learning                      | family   | Yasmin   |
			| Somthing everyone can do      | family   |          |
			| Somthing else everyone can do | family   |          |
		When User select item 3 from tasks of 'family'
		When User assign selected task to 'Yasmin' for 'family'
		When User select item 2 from tasks of 'family'
		When User assign selected task to 'Mania' for 'family'
		When User select item 1 from tasks of 'family'
		When User assign selected task to 'Hossein' for 'family'
		When User select item 1 from tasks of 'Hossein'
		When User set location 'Home' on selected task for 'Hossein'
		When User select item 1 from tasks of 'Yasmin'
		When User set location 'Work' on selected task for 'Yasmin'
		When User select item 1 from tasks of 'Mania'
		When User add 'Urgent' to tag 0 on selected task for 'Mania'
		When User add 'Any tag' to tag 1 on selected task for 'Mania'
		When User add 'Any other tag' to tag 1 on selected task for 'Mania'
		Then I should see the following group board for 'family':
			| Id      |
			| family  |
			| Hossein |
			| Mania   |
			| Yasmin  |
		Then I should see the following group 'All'
			| Group   | Member  |
			| family  | family  |
			| family  | Hossein |
			| family  | Mania   |
			| family  | Yasmin  |
			| Hossein | Hossein |
			| Mania   | Mania   |
			| Yasmin  | Yasmin  |
