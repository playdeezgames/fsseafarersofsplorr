# SeafarersOfSplorr

# Order of TDD Operations

1. When it doesn't build... fix the build!
2. When any code does not have test coverage... stub a test to cover the uncovered code!
4. When a test is failing because it is a stubbed out test... finish writing the test!
5. When a test is failing because it is not a good test... fix the test!
6. When a test is failing because the code is wrong... fix the code!
7. When all tests are passing and the code isn't checked in... check in the code!
8. When all tests are passing and the code is checked in and tests needs refactoring... refactor tests and check in again!
9. When all tests are passing and the code is checked in and no test needs refactoring but code needs refactoring... refactor code as needed and check in again!
10. When all tests are passing and the code is checked in and nothing needs refactoring... stub out something new and start all over!