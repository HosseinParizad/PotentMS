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
		| Hossein | Hossein  |           |
		| Mania   | Hossein  |           |
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  |           |
		| Mania   | Hossein  |           |
		| Yasmin  | Hossein  |           |

Scenario: Update group
	Then I send the following Group list:
		| Group   |
		| Hossein |
	Then Use add member 'Mania' to group 'Hossein'
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  |           |
		| Mania   | Hossein  |           |
		| Yasmin  | Hossein  |           |
	When I update group 'Hossein' to 'Asghar'
	Then I should see the following Group list:
		| Text   | GroupKey | MemberKey |
		| Asghar | Asghar   |           |
		| Mania  | Asghar   |           |
		| Yasmin | Asghar   |           |

Scenario: Delete group
	Then I send the following Group list:
		| Group   |
		| Hossein |
		| Mania   |
	Then Use add member 'Mania' to group 'Hossein'
	Then Use add member 'Yasmin' to group 'Hossein'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Hossein | Hossein  |           |
		| Mania   | Hossein  |           |
		| Yasmin  | Hossein  |           |
	When I delete group 'Hossein'
	Then I should see the following Group list:
		| Text   | GroupKey |
	Then I should see the following Group list:
		| Text  | GroupKey | MemberKey |
		| Mania | Mania    | Mania     |

Scenario: Member filter
	Then I send the following Group list:
		| Group  |
		| Group1 |
		| Group2 |
	Then I should see the following Group list:
		| Text   | GroupKey | MemberKey |
		| Group1 | Group1   | Group1    |
		| Group2 | Group2   | Group2    |
	Then Use add member 'Hossein' to group 'Group1'
	Then Use add member 'Hossein' to group 'Group2'
	Then I should see the following Group list:
		| Text    | GroupKey | MemberKey |
		| Group1  | Group1   |           |
		| Hossein | Group1   |           |
		| Group2  | Group2   |           |
		| Hossein | Group2   |           |
		| Hossein | Group1   | Hossein   |
		| Hossein | Group2   | Hossein   |
		| Hossein |          | Hossein   |
		| Hossein |          | Hossein   |