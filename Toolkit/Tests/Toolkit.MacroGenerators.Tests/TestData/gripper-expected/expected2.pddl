(:action pick-pick-move-drop-drop
    :parameters (?ball1 ?ball2 ?rooma ?roomb ?left ?right)
    :precondition  (and 
			(at-robby ?rooma) 
			(free ?left)
			(free ?right)
			(ball ?ball1)
			(ball ?ball2)
			(room ?rooma)
			(room ?roomb)
			(gripper ?left)
			(gripper ?right)
			(at ?ball1 ?rooma)
			(at ?ball2 ?rooma)
			)
    :effect (and 
			(at-robby ?roomb)
			(not (at-robby ?rooma))
			(not (at ?ball1 ?rooma))
			(not (at ?ball2 ?rooma))
			(at ?ball1 ?roomb)
			(at ?ball2 ?roomb)
		)
	)