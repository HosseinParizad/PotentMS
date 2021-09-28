Feature: iMemory
	Memory main engine 

Scenario: Memorize
	Given User Ask to Memorize 'Test' for 'Ali' in group 'Ali'
	Then I should see the following Memorize list:
		| Text | GroupKey | MemberKey |
		| Test | Ali      | Ali       |

Scenario: Memorize other item
	Given User Ask to Memorize 'Test' for 'Ali' in group 'Ali'
	Given User Ask to Memorize 'Something else' for 'Ali' in group 'Ali'
	Then I should see the following Memorize list:
		| Text           | GroupKey | MemberKey |
		| Test           | Ali      | Ali       |
		| Something else | Ali      | Ali       |

Scenario: learnt
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      | Ali       |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen  | Ali      | Ali       |
	Then I should see the following Memorize list:
		| Id                                   | Text | GroupKey | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      | Ali       |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen  | Ali      | Ali       |
	Given User tell I Memorize '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali' in 'Ali'
	Then I should see the following Memorize list:
		| Text         | GroupKey | MemberKey |
		| Book         | Ali      | Ali       |
		| Pen (Stage1) | Ali      | Ali       |
	Given User tell I Memorize '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali' in 'Ali'
	Then I should see memorize list after 3 days:
		| Text         | GroupKey | MemberKey |
		| Book         | Ali      | Ali       |
		| Pen (Stage2) | Ali      | Ali       |

Scenario: learnt child
	Then I send the following Memorize list:
		| Id                                   | Text   | GroupKey | ParentId                             | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book   | Ali      |                                      | Ali       |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen    | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Ali       |
		| A51C422A-78E5-4A97-BBCF-FB87EB903504 | Pencel | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Ali       |
	Then I should see the following Memorize list:
		| Id                                   | Text | GroupKey | Children | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      | 2        | Ali       |
	Given User tell I Memorize '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali' in 'Ali'
	Then I should see the following Memorize list:
		| Text | GroupKey | Children | MemberKey |
		| Book | Ali      | 2        | Ali       |
	Given User tell I Memorize '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali' in 'Ali'
	Then I should see the following Memorize list:
		| Text | GroupKey | Children | MemberKey |
		| Book | Ali      | 1        | Ali       |
	Then I should see memorize list after 1 days:
		| Text | GroupKey | Children | MemberKey |
		| Book | Ali      | 2        | Ali       |

Scenario: Delete
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      | Ali       |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen  | Ali      | Ali       |
	Then I should see the following Memorize list:
		| Text | GroupKey | MemberKey |
		| Book | Ali      | Ali       |
		| Pen  | Ali      | Ali       |
	Then User delete memory '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	Then I should see the following Memorize list:
		| Text | GroupKey | MemberKey |
		| Pen  | Ali      | Ali       |

Scenario: Delete child
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey | ParentId                             | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      |                                      | Ali       |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen  | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Ali       |
	Then I should see the following Memorize list:
		| Id                                   | Text | GroupKey | Children | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      | 1        | Ali       |
	Then User delete memory '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali' in 'Ali'
	Then I should see the following Memorize list:
		| Text | GroupKey | Children | MemberKey |
		| Book | Ali      | 0        | Ali       |

#| Id
#| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3
#| 1D7335DA-7013-482A-A23F-62CB24939EE6
#| A51C422A-78E5-4A97-BBCF-FB87EB903504
Scenario: Memorize stages
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey | MemberKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      | Ali       |
	Then I should see the following Memorize list:
		| Text | GroupKey | MemberKey |
		| Book | Ali      | Ali       |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	# 1
	Then I should see memorize list after 1 days:
		| Text          | GroupKey | MemberKey |
		| Book (Stage1) | Ali      | Ali       |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	Then I should see memorize list after 1 days:
		| Text          | GroupKey | MemberKey |
		| Book (Stage2) | Ali      | Ali       |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	# 3 + 1
	Then I should see memorize list after 4 days:
		| Text          | GroupKey | MemberKey |
		| Book (Stage3) | Ali      | Ali       |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	# 7 + 3 + 1
	Then I should see memorize list after 11 days:
		| Text          | GroupKey | MemberKey |
		| Book (Stage4) | Ali      | Ali       |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	# 14 + 7 + 3 + 1
	Then I should see memorize list after 25 days:
		| Text          | GroupKey | MemberKey |
		| Book (Stage5) | Ali      | Ali       |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	# 30 + 14 + 7 + 3 + 1
	Then I should see memorize list after 55 days:
		| Text          | GroupKey | MemberKey |
		| Book (Stage6) | Ali      | Ali       |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali' in 'Ali'
	# :) + 30 + 14 + 7 + 3 + 1 does not show anymore
	Then I should see memorize list after 1000 days:
		| Text          | GroupKey |MemberKey|
#| Book (Stage6) | Ali      |