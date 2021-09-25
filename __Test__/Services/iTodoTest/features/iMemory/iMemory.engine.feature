Feature: iMemory
	Memory main engine 

Scenario: Memorize
	Given User Ask to Memorize 'Test' for 'Ali'
	Then I should see the following Memorize list:
		| Text          | GroupKey |
		| Test (Stage1) | Ali      |

Scenario: Memorize other item
	Given User Ask to Memorize 'Test' for 'Ali'
	Given User Ask to Memorize 'Something else' for 'Ali'
	Then I should see the following Memorize list:
		| Text                    | GroupKey |
		| Test (Stage1)           | Ali      |
		| Something else (Stage1) | Ali      |

Scenario: learnt
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen  | Ali      |
	Then I should see the following Memorize list:
		| Id                                   | Text          | GroupKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book (Stage1) | Ali      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen (Stage1)  | Ali      |
	Given User tell I Memorize '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali'
	Then I should see the following Memorize list:
		| Text          | GroupKey |
		| Book (Stage1) | Ali      |
	Then I should see after 3 days:
		| Text          | GroupKey |
		| Book (Stage1) | Ali      |
		| Pen (Stage2)  | Ali      |

Scenario: learnt child
	Then I send the following Memorize list:
		| Id                                   | Text   | GroupKey | ParentId                             |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book   | Ali      |                                      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen    | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 |
		| A51C422A-78E5-4A97-BBCF-FB87EB903504 | Pencel | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 |
	Then I should see the following Memorize list:
		| Id                                   | Text          | GroupKey | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book (Stage1) | Ali      | 2        |
	Given User tell I Memorize '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali'
	Then I should see the following Memorize list:
		| Text          | GroupKey | Children |
		| Book (Stage1) | Ali      | 1        |
	Then I should see after 1 days:
		| Text          | GroupKey | Children |
		| Book (Stage1) | Ali      | 2        |

Scenario: Delete
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen  | Ali      |
	Then I should see the following Memorize list:
		| Text          | GroupKey |
		| Book (Stage1) | Ali      |
		| Pen (Stage1)  | Ali      |
	Then User delete memory '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	Then I should see the following Memorize list:
		| Text         | GroupKey |
		| Pen (Stage1) | Ali      |

Scenario: Delete child
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey | ParentId                             |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      |                                      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Pen  | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 |
	Then I should see the following Memorize list:
		| Id                                   | Text          | GroupKey | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book (Stage1) | Ali      | 1        |
	Then User delete memory '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali'
	Then I should see the following Memorize list:
		| Text          | GroupKey | Children |
		| Book (Stage1) | Ali      | 0        |

#| Id
#| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3
#| 1D7335DA-7013-482A-A23F-62CB24939EE6
#| A51C422A-78E5-4A97-BBCF-FB87EB903504
Scenario: Memorize stages
	Then I send the following Memorize list:
		| Id                                   | Text | GroupKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Book | Ali      |
	Then I should see the following Memorize list:
		| Text          | GroupKey |
		| Book (Stage1) | Ali      |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	Then I should see after 1 days:
		| Text          | GroupKey |
		| Book (Stage2) | Ali      |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	# 3 + 1
	Then I should see after 4 days:
		| Text          | GroupKey |
		| Book (Stage3) | Ali      |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	# 7 + 3 + 1
	Then I should see after 11 days:
		| Text          | GroupKey |
		| Book (Stage4) | Ali      |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	# 14 + 7 + 3 + 1
	Then I should see after 25 days:
		| Text          | GroupKey |
		| Book (Stage5) | Ali      |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	# 30 + 14 + 7 + 3 + 1
	Then I should see after 55 days:
		| Text          | GroupKey |
		| Book (Stage6) | Ali      |
	Given User tell I Memorize '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	# :) + 30 + 14 + 7 + 3 + 1 does not show anymore
	Then I should see after 1000 days:
		| Text          | GroupKey |
#| Book (Stage6) | Ali      |