Feature: Location

	Scenario: Setting location
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set location 'Home' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Locations |
			| Watch cat videos on YouTube all day | Ali      | ["Home"]  |


	Scenario: Setting multi locations
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set location 'Home,Roof' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Locations       |
			| Watch cat videos on YouTube all day | Ali      | ["Home","Roof"] |

	Scenario: Setting multi locations with sub location
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set location 'Home,Home -> Yard -> Garden' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Locations                         |
			| Watch cat videos on YouTube all day | Ali      | ["Home","Home -> Yard -> Garden"] |

	Scenario: Should be able to sort by location
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
			| Watch dog videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User set location 'Home' on selected task for 'Ali'
		When User select item 2 from tasks of 'Ali'
		When User set location 'Work' on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Locations |
			| Watch cat videos on YouTube all day | Ali      | ["Home"]  |
			| Watch dog videos on YouTube all day | Ali      | ["Work"]  |
		When User 'Ali' go to 'Work'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Locations |
			| Watch dog videos on YouTube all day | Ali      | ["Work"]  |
			| Watch cat videos on YouTube all day | Ali      | ["Home"]  |
		When User 'Ali' go to 'Home'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Locations |
			| Watch cat videos on YouTube all day | Ali      | ["Home"]  |
			| Watch dog videos on YouTube all day | Ali      | ["Work"]  |
