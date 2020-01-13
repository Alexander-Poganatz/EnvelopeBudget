# EnvelopeBudget
An envelope budgeting app that I made to learn about functional programming and help plan spending

# Console App Commands

* new [envelope_name] name of the envolope, no whitespaces
* delete [envelope_name] deletes the envelope from the list
* add [envelope_name] [dollar amount] [yyyy-mm-dd] [description]: Adds a transaction to the dollar amount, use negative dollar values to take money out of the envelope
* remove [envelope_name] [transaction index number] removes a transaction based off the index, use the show command to see the index on the left. indexes are updated after each add and remove command.
* show [envelope_name] shows the transactions in an envelope
* summary or sum: show all envelopes and the balance
* load [file_path] loads the given file
* save or ctrl-s: Saves the session to the default file. ctrl-s is not tested in linux yet. 
* quit or exit or q or e: exit the application

The default file path is ./budget.json 

ctrl-s still needs to have entered pressed after it.