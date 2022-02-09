


items = [41, 15, 17, 32, 18, 28, 77, 54] # 8 items.

change_made = True

while change_made:  # Used to optimise the code to stop any more passes from happening if no changes are made.
    for i in range(len(items)):        
        
        for j in range(len(items) - 1):
            change_made = False
            print(items)
            if items[j] > items[j + 1]:
                change_made = True
                print(f"{j} is j")
                temp = items[j]
                print(f"{temp} is being moved!")
                items[j] = items[j + 1]
                items[j + 1] = temp
            
            if not change_made:
                print("No changes made!")
                break
                
        
        if not change_made:
            break
    

