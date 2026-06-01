# CHANGELOG

## 01-06-2026

### ADDED
- Clear Cart function
- Checkout function
- EndDate calculations
- Register button to only be once person is logged in (so they can add employees)
- In ShoppingCartService added a function for clear cart
- Employees require a cashierID to clear the cart
- Added a function to calculate the end date of a contract based on the start date and duration of the contract
- Scaffolded item for Logout (needed to fix the logout bug)
- Javascript function to toggle search bars under each header column in the Manufacturers page
- A new Relational Database to establish the relationship between the product and receipt (order)
- Added an EmployeeID in the SeedData for the initial admin account
- Added two Tasks in ShoppingCarts Controller for checkout and clearing of cart
- User can now enter a **FULL** barcode and add the item in the cart
- If the product is already in the cart, the quantity will be increased by 1 instead of adding a new line item in the cart

### FIXED
- Fixed logout function
- Duration of contract input area is now fisible once a contract type "FIXED" is selected
- Shopping cart is associated to a user upon creation
- Fixed buggy UI in "Cart"


### TO-DO
- Make the search bars in the Manufacturers page functional
- Make the search bars in the Products page functional
- Make the search bars in the Categories page functional
- Add a function to add a product via the barcode image