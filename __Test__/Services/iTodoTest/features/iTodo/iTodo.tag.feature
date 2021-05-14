Feature: Tag

	Scenario: Adding Tag
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags     |
			| Watch cat videos on YouTube all day | Ali      | [{"tagParentKey":"0","value":"Home"}] |

	Scenario: Should be able to set tag
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags                                |
			| Watch cat videos on YouTube all day | Ali      | [{"tagParentKey":"0","value":"Home"}] |
		When User add 'Garden' to tag 0 on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags              |
			| Watch cat videos on YouTube all day | Ali      | [{"tagParentKey":"0","value":"Home,Garden"}] |

	Scenario: Should not add same tag twice
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags                                  |
			| Watch cat videos on YouTube all day | Ali      | [{"tagParentKey":"0","value":"Home"}] |
		When User add 'Home' to tag 0 on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags                                  |
			| Watch cat videos on YouTube all day | Ali      | [{"tagParentKey":"0","value":"Home"}] |

	Scenario: Should not add same tag twice bas message
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home,Home' to tag 0 on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags                                  |
			| Watch cat videos on YouTube all day | Ali      | [{"tagParentKey":"0","value":"Home"}] |

	Scenario: Adding Tag with defferent Parent Key
		Given  I send the following task:
			| TaskDesc                            | GroupKey |
			| Watch cat videos on YouTube all day | Ali      |
		When User select item 1 from tasks of 'Ali'
		When User add 'Home' to tag 0 on selected task for 'Ali'
		When User add 'Home' to tag 1 on selected task for 'Ali'
		Then I should see the following todo list:
			| TaskDesc                            | GroupKey | Tags                                  |
			| Watch cat videos on YouTube all day | Ali | [{"tagParentKey":"0","value":"Home"},{"tagParentKey":"1","value":"Home"}] |