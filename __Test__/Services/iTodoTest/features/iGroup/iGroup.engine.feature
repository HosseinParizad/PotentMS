Feature: iGroup
	Group main engine 

Scenario: Insert group
	Then I send the following Group list:
		| Group   |
		| Hossein |
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  | Hossein   |

Scenario: Add member
	Then I send the following Group list:
		| Group   |
		| Hossein |
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  | Hossein   |
	Then Use add member 'Mania' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  | Hossein   |
		| Mania   | Hossein  | Hossein   |
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  | Hossein   |
		| Mania   | Hossein  | Hossein   |
		| Yasmin  | Hossein  | Hossein   |

Scenario: Update group
	Then I send the following Group list:
		| Group   |
		| Hossein |
	Then Use add member 'Mania' to group 'Hossein'
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  | Hossein   |
		| Mania   | Hossein  | Hossein   |
		| Yasmin  | Hossein  | Hossein   |
	When I update group 'Hossein' to 'Asghar'
	Then I should see the following Group list:
		| Text   | GroupKey | MemberKey |
		| Asghar | Asghar   | Hossein   |
		| Mania  | Asghar   | Hossein   |
		| Yasmin | Asghar   | Hossein   |

Scenario: Delete group
	Then I send the following Group list:
		| Group   |
		| Hossein |
		| Mania   |
	Then Use add member 'Mania' to group 'Hossein'
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  | Hossein   |
		| Mania   | Hossein  | Hossein   |
		| Yasmin  | Hossein  | Hossein   |
	When I delete group 'Hossein'
	Then I should see the following Group list:
		| Text   | GroupKey |
	Then I should see the following Group list:
		| Text  | GroupKey | MemberKey |
		| Mania | Mania    | Mania     |