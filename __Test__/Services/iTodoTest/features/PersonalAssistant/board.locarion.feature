Feature: PersonalAssistant.Location

	Scenario: Should see the location section
		Then I should see the following board for 'Me':
			| Text          | Badges |
			| Goal          | []     |
			| Tag           | []     |
			| UsedLocations | []     |

	# TODO: need to find where is best please to order task by location (todo, dashboard, ...)
	# Scenario: Should get location event
	# 	Given  I send the following task:
	# 		| TaskDesc  | GroupKey |
	# 		| Something | Me       |
	# 	When User select item 1 from tasks of 'Me'
	# 	When User 'Me' go to 'Home'
	# 	Then I should see the following board for 'Me':
	# 		| Text            | CurrentLocation |
	# 		| CurrentLocation | Home            |

	Scenario: Should be able see UsedLocations
		Given  I send the following task:
			| TaskDesc  | GroupKey |
			| Buy carot | Me       |
			| Eat carot | Me       |
		When User select item 1 from tasks of 'Me'
		When User set location 'Shop -> Woolies' on selected task for 'Me'
		When User select item 2 from tasks of 'Me'
		When User set location 'Home -> dining room -> table' on selected task for 'Me'
		Then I should see the following todo list:
			| TaskDesc  | GroupKey | Locations                        |
			| Buy carot | Me       | ["Shop -> Woolies"]              |
			| Eat carot | Me       | ["Home -> dining room -> table"] |
		Then I should see the following board for 'Me':
			| Text          | Badges                                             |
			| Goal          | []                                                 |
			| Tag           | []                                                 |
			| UsedLocations | ["Shop -> Woolies","Home -> dining room -> table"] |
