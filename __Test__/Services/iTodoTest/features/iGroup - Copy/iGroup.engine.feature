Feature: iGroup
	Group main engine 

Scenario: Insert group
	Then I send the following Group list:
		| Group   |
		| Hossein |
	Then I should see the following Group list:
		| Text    | GroupKey |
		| Hossein | Hossein  |

Scenario: Add member
	Then I send the following Group list:
		| Group   |
		| Hossein |
	Then I should see the following Group list:
		| Text    | GroupKey |
		| Hossein | Hossein  |
	Then Use add member 'Mania' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey |
		| Hossein | Hossein  |
		| Mania   | Hossein  |
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey |
		| Hossein | Hossein  |
		| Mania   | Hossein  |
		| Yasmin  | Hossein  |

Scenario: Update group
	Then I send the following Group list:
		| Group   |
		| Hossein |
	Then Use add member 'Mania' to group 'Hossein'
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey |
		| Hossein | Hossein  |
		| Mania   | Hossein  |
		| Yasmin  | Hossein  |
	When I update group 'Hossein' to 'Asghar'
	Then I should see the following Group list:
		| Text   | GroupKey |
		| Asghar | Asghar   |
		| Mania  | Asghar   |
		| Yasmin | Asghar   |

Scenario: Delete group
	Then I send the following Group list:
		| Group   |
		| Hossein |
		| Mania   |
	Then Use add member 'Mania' to group 'Hossein'
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey |
		| Hossein | Hossein  |
		| Mania   | Hossein  |
		| Yasmin  | Hossein  |
	When I delete group 'Hossein'
	Then I should see the following Group list:
		| Text   | GroupKey |
	Then I should see the following Group list:
		| Text  | GroupKey |
		| Mania | Mania    |