(:action pick
	:parameters (ball1 roomb right)
	:precondition  
		(and  
			(ball ball1) 
			(room roomb) 
			(gripper right)
			(at ball1 roomb) 
			(at-robby roomb) 
			(free right)
		)
	:effect 
		(and 
			(carry ball1 right)
		    (not (at ball1 roomb)) 
		    (not (free right))
		)
	)