(:action LOAD-TRUCK
    :parameters (item1 truck1 location1)
    :precondition
        (and 
            (OBJ item1) 
            (TRUCK truck1)
            (LOCATION location1)
            (at truck1 location1) 
            (at item1 location1)
        )
    :effect
        (and 
            (not (at item1 location1)) 
            (in item1 truck1)
        )
    )