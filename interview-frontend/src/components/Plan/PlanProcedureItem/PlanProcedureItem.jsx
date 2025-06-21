import React, { useEffect, useState } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({ procedure, users,planProcedure,handleAddUserToProcedurePlan,handleRemoveUserFromProcedurePlan }) => {
    

    const getUsersFromPlanProcedure =() => {      
        if (!planProcedure || !planProcedure.planProcedureUsers) return [];
        
        return planProcedure.planProcedureUsers.map(ppu => ({
            label: ppu.user.name,
            value: ppu.user.userId
        }));
    }

    const [selectedUsers, setSelectedUsers] = useState(null);

    useEffect(()=>{
       const userData= getUsersFromPlanProcedure();
       setSelectedUsers(userData);
    },[]);

    const handleAssignUserToProcedure = (e) => {
        setSelectedUsers(e);
        const assignedUserIds = new Set(
            planProcedure.planProcedureUsers.map(ppu => ppu.userId)
          );
        
          const selectedUserIds = new Set(
            e.map(ppu => ppu.value)
          );
          
          const addedUser = e.filter(u => !assignedUserIds.has(u.value));
          const removedUser = planProcedure.planProcedureUsers.filter(ppu => !selectedUserIds.has(ppu.userId));
            if(addedUser && addedUser.length > 0) {
                handleAddUserToProcedurePlan(planProcedure.procedure,addedUser[0].value);
            }
            if(removedUser && removedUser.length > 0){
                handleRemoveUserFromProcedurePlan(planProcedure.procedure,removedUser[0].userId);
            }
            if(e.length == 0){
                handleRemoveUserFromProcedurePlan(planProcedure.procedure);
            }

    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
