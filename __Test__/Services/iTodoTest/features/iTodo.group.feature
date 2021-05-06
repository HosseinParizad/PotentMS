Feature: Group

	Scenario: Adding group
		Given  Send an email '@me' to create group
		Then I should see the following groups:
			| Group | Member |
			| @me   | @me    |

	Scenario: User can be member of a group
		Given  Send an email '@me' to create group
		Given  Send an email '@family' to create group
		Given  Add '@me' as member of '@family'
		Then I should see the following groups:
			| Group   | Member  |
			| @me     | @me     |
			| @family | @family |
			| @family | @me     |

	Scenario: Group can have more than one member
		Given  Send an email '@family' to create group
		Given  Add '@me' as member of '@family'
		Given  Add '@you' as member of '@family'
		Then I should see the following groups:
			| Group   | Member  |
			| @family | @family |
			| @family | @me     |
			| @family | @you    |
			| @me     | @me     |
			| @you    | @you    |

	Scenario: Adding task should add group if not exits
		Given I send the following task:
			| TaskDesc       | GroupKey |
			| Watch somthing | @me      |
		Then I should see the following groups:
			| Group | Member |
			| @me   | @me    |
		Given  I send the following task:
			| TaskDesc            | GroupKey |
			| Watch somthing else | @me      |
		Then I should see the following groups:
			| Group | Member |
			| @me   | @me    |

	Scenario: Adding task should add group if not exits 2
		Given I send the following task:
			| TaskDesc            | GroupKey |
			| Watch somthing      | @me      |
			| Watch somthing else | @you     |
		Then I should see the following groups:
			| Group | Member |
			| @me   | @me    |
			| @you  | @you   |
