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

    const [selectedUsers, setSelectedUsers] = useState([]);

    useEffect(()=>{
       const userData= getUsersFromPlanProcedure();
       setSelectedUsers(userData);
    },[planProcedure]);

    const handleAssignUserToProcedure = (e) => {
    setSelectedUsers(e || []);

    const assignedUserIds = new Set(planProcedure?.planProcedureUsers?.map(ppu => ppu.userId));
    const selectedUserIds = new Set((e || []).map(ppu => ppu.value));

    const addedUsers = (e || []).filter(u => !assignedUserIds.has(u.value));
    const removedUsers = planProcedure?.planProcedureUsers?.filter(ppu => !selectedUserIds.has(ppu.userId));

    if (addedUsers?.length) {
        addedUsers.forEach(user =>
            handleAddUserToProcedurePlan(planProcedure.procedure, user.value)
        );
    }

    if (e.length === 0) {
        // Remove all users
        handleRemoveUserFromProcedurePlan(planProcedure.procedure);
    } else if (removedUsers?.length) {
        // Remove specific user
        removedUsers.forEach(user =>
            handleRemoveUserFromProcedurePlan(planProcedure.procedure, user.userId)
        );
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
