Feature: Group

Scenario: Adding group
	Given Send an email '@me' to create group
	And Wait 1000
	Then I should see the following groups:
		| Group |
		| @me   |

Scenario: User can be member of a group
	Given Send an email '@me' to create group
	Given Send an email '@family' to create group
	Given Add '@me' as member of '@family'
	Then I should see the following group 'All'
		| Group   | Member  |
		| @me     | @me     |
		| @family | @family |
		| @family | @me     |

Scenario: Group can have more than one member
	Given Send an email '@family' to create group
	And Add '@me' as member of '@family'
	And Add '@you' as member of '@family'
	And Wait 1000
	Then I should see the following group 'All'
		| Group   | Member  |
		| @family | @family |
		| @family | @me     |
		| @family | @you    |
		| @me     | @me     |
		| @you    | @you    |

Scenario: Adding task should add group if not exits
	Given Someone send the following task and group:
		| TaskDesc       | GroupKey |
		| Watch somthing | @me      |
	Then I should see the following groups:
		| Group | Member |
		| @me   | @me    |
	Given I send the following task:
		| TaskDesc            | GroupKey |
		| Watch somthing else | @me      |
	Then I should see the following groups:
		| Group | Member |
		| @me   | @me    |

Scenario: Adding task should add group if not exits 2
	Given Someone send the following task and group:
		| TaskDesc            | GroupKey |
		| Watch somthing      | @me      |
		| Watch somthing else | @you     |
	Then I should see the following group 'All'
		| Group | Member |
		| @me   | @me    |
		| @you  | @you   |