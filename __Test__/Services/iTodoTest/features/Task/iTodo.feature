Feature: Todo list

Scenario: Should be able to create task
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |
	Then I should see the following todo list directly:
		| Id                                   | Text                                | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | Ali      |          |

Scenario: Split by group
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | AAA      |          |
	Then I should see the following todo list directly:
		| Id                                   | Text                                | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Watch dog videos on YouTube all day | AAA      |          |

Scenario: Update
	Given I send to do items messages:
		| Id                                   | Description                         | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
	Then I should see the following todo list directly:
		| Id                                   | Text                                | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Watch cat videos on YouTube all day | Ali      |          |
	When Update '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' task description to 'Updated task :)' for 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text            | GroupKey |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Updated task :) | Ali      |

Scenario: Move task
	Given I send to do items messages:
		| Id                                   | Description  | GroupKey | ParentId                             |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Parent 1     | Ali      |                                      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Parent 2     | Ali      |                                      |
		| A51C422A-78E5-4A97-BBCF-FB87EB903504 | Stopit child | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 |
	Then I should see the following todo list directly:
		| Id                                   | Text     | GroupKey | ParentId | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Parent 1 | Ali      |          | 1        |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Parent 2 | Ali      |          | 0        |
	#| A51C422A-78E5-4A97-BBCF-FB87EB903504 | Stopit child | Ali      | 1D7335DA-7013-482A-A23F-62CB24939EE6 |
	When Move task 'A51C422A-78E5-4A97-BBCF-FB87EB903504' to '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text     | GroupKey | ParentId | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Parent 1 | Ali      |          | 0        |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Parent 2 | Ali      |          | 1        |

#| A51C422A-78E5-4A97-BBCF-FB87EB903504 | Stopit child | Ali      | 1D7335DA-7013-482A-A23F-62CB24939EE6 |
Scenario: Delete task
	Given I send to do items messages:
		| Id                                   | Description | GroupKey | ParentId |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1      | Ali      |          |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2      | Ali      |          |
	Then I should see the following todo list directly:
		| Id                                   | Text   | GroupKey | ParentId | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1 | Ali      |          | 0        |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2 | Ali      |          | 0        |
	When Delete task '1D7335DA-7013-482A-A23F-62CB24939EE6' for 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text   | GroupKey | ParentId | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1 | Ali      |          | 0        |

Scenario: Delete task with child
	Given I send to do items messages:
		| Id                                   | Description  | GroupKey | ParentId                             |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1       | Ali      |                                      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2       | Ali      |                                      |
		| A51C422A-78E5-4A97-BBCF-FB87EB903504 | Stopit child | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 |
	Then I should see the following todo list directly:
		| Id                                   | Text   | GroupKey | ParentId | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1 | Ali      |          | 1        |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2 | Ali      |          | 0        |
	When Delete task '47399CA8-C533-413C-A2A9-BAF5CDB85AE3' for 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text   | GroupKey | ParentId | Children |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2 | Ali      |          | 0        |

Scenario: Delete child
	Given I send to do items messages:
		| Id                                   | Description  | GroupKey | ParentId                             |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1       | Ali      |                                      |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2       | Ali      |                                      |
		| A51C422A-78E5-4A97-BBCF-FB87EB903504 | Stopit child | Ali      | 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 |
	Then I should see the following todo list directly:
		| Id                                   | Text   | GroupKey | ParentId | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1 | Ali      |          | 1        |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2 | Ali      |          | 0        |
	When Delete task 'A51C422A-78E5-4A97-BBCF-FB87EB903504' for 'Ali'
	Then I should see the following todo list directly:
		| Id                                   | Text   | GroupKey | ParentId | Children |
		| 47399CA8-C533-413C-A2A9-BAF5CDB85AE3 | Task 1 | Ali      |          | 0        |
		| 1D7335DA-7013-482A-A23F-62CB24939EE6 | Task 2 | Ali      |          | 0        |