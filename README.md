# SeafarersOfSplorr

# Order of TDD Operations

1. When I have untested code (even for stubbed out functionality)
   1. Stub out some unit tests against the new code (typically, throwing a not implemented exception)
2. When a unit tests fails...
   1. ...because it is poorly written or inaccurate:
      1. Fix the test
      2. Retest
   2. ...because the subject code is not implemented/inaccurately implemented:
      1. Implement/fix the subject code
      2. Retest
3. When all unit tests pass or there are no unit tests:
   1. Ensure the code is checked in!
   2. Stub out a new piece of functionality (typically, throws a not implemented exception)
