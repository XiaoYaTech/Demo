/// <reference path="../Libs/moment/moment.js" />
/// <reference path="../Utils/Utils.js" />
!function () {
    angular.module("project.list", [
        "mcd.am.service.taskwork",
        "mcd.am.services.projectUsers",
        "mcd.am.services.flow",
        "mcd.am.services.project",
        "mcd.am.services.tempClosure"
    ]).controller('projectListCtrl', [
        '$scope',
        "$http",
        "$modal",
        "$window",
        "$selectStore",
        "taskWorkService",
        "flowService",
        "projectService",
        "messager",
        "approveDialogService",
        'projectStatusChangeService',
        function ($scope, $http, $modal, $window, $selectStore, taskWorkService, flowService, projectService, messager, approveDialogService, projectStatusChangeService) {
            $scope.pageIndex = 1;
            $scope.pageSize = 10;
            $scope.totalItems = 0;
            $scope.list = [];
            $scope.searchCondition = new projectSearchCondition();
            $scope.conditionTable = [];
            $scope.isShowSearchValues = false;
            $scope.isLoading = true;
            if (window["currentUser"]) {
                $scope.showAdvanceSearch = $.inArray("am_pl_as", window["currentUser"].RightCodes) >= 0;
                $scope.showSearch = $.inArray("am_pl_search", window["currentUser"].RightCodes) >= 0;
            }

            $http.get(Utils.ServiceURI.Address() + "api/projectusers/GetPackageHoldingRoleUsers").success(function (data) {
                if ($.grep(data || [], function (user, i) {
                    return user.Code == window["currentUser"].Code;
                }).length != 0)
                    $scope.userHoldable = true;
                else
                    $scope.userHoldable = false;
            });

            $scope.dateOptions = {
                formatYear: 'yy',
                startingDay: 1
            };

            $scope.projectTypes = [
                { key: "Closure", value: "Closure" },
                { key: "MajorLease", value: "MajorLease" },
                { key: "TempClosure", value: "TempClosure" },
                { key: "Rebuild", value: "Rebuild" },
                { key: "Reimage", value: "Reimage" },
                { key: "Renewal", value: "Renewal" }
            ];
            $scope.holdStatuses = ["Yes", "No"];
            $scope.pagging = function () {
                $scope.conditionTable = [];
                $scope.ajaxFinished = false;
                for (var prop in $scope.searchCondition) {
                    var value = $scope.searchCondition[prop];
                    if (prop.indexOf("_") != 0 && !!value) {
                        if (typeof value == "object" && value instanceof Date) {
                            value = moment(value).format("YYYY-MM-DD");
                        }
                        if (typeof value == "object" && value instanceof Array) {
                            value = value.join(" , ");
                        }
                        $scope.conditionTable.push({
                            key: prop,
                            value: value
                        });
                    }
                }
                if ($scope.conditionTable.length > 0) {
                    $scope.isShowSearchValues = true;
                } else {
                    $scope.isShowSearchValues = false;
                }

                var url = Utils.ServiceURI.Address() + "api/project/" + $scope.pageIndex + "/" + $scope.pageSize;
                //$.ajax({
                //    url: url,
                //    type: "POST",
                //    data: $scope.searchCondition
                //}).success(function (data) {
                //    $scope.list = data.List;
                //    $scope.totalItems = data.TotalItems;
                //    $scope.ajaxFinished = true;
                //    $scope.$apply();
                //});
                $http.post(url, $scope.searchCondition).success(function (data) {
                    $scope.list = data.List;
                    $scope.totalItems = data.TotalItems;
                    $scope.ajaxFinished = true;
                    $scope.$apply();
                });
            };
            $scope.$watch("pageIndex", function (val) {
                if (!!val) {
                    $scope.pagging();
                };
            });
            $scope.search = function () {
                $scope.pageIndex = 1;
                $scope.pagging();
            };
            $scope.openDatepicker = function ($event, dateOpenTag) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.searchCondition[dateOpenTag] = true;
            };
            $scope.removeSearchCondition = function (key) {
                $scope.searchCondition[key] = null;
                angular.forEach($scope.conditionTable, function (cdtItem, i) {
                    if (cdtItem.key == key) {
                        $scope.conditionTable.splice(i, 1);
                    }
                });
                if ($scope.conditionTable.length > 0) {
                    $scope.isShowSearchValues = true;
                } else {
                    $scope.isShowSearchValues = false;
                }
            };
            $scope.changeProjectStatus = function (project) {
                projectStatusChangeService.open(project.ProjectId, project.FlowCode, project.Status, project.USCode).then(function (result) {
                    messager.showMessage("[[[成功变更项目状态]]]", 'fa-check c_green').then(function () {
                        $scope.search();
                    });
                });
            };
            $scope.pending = function (project) {
                messager.confirm("[[[确定中止项目吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        projectService.pending(project.ProjectId).$promise.then(function () {
                            messager.showMessage("[[[成功终止项目]]]", "fa-check c_green");
                            project.Status = 3;
                        }, function () {
                            messager.showMessage("[[[终止项目没有成功]]]", "fa-warning c_red");
                        });
                    };
                });
            };
            $scope.resume = function (project) {
                messager.confirm("[[[确定恢复项目吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        projectService.resume(project.ProjectId).$promise.then(function () {
                            messager.showMessage("[[[成功恢复项目]]]", "fa-check c_green");
                            project.Status = 0;
                        }, function () {
                            messager.showMessage("[[[恢复项目没有成功]]]", "fa-warning c_red");
                        });
                    };
                });
            };
            $scope.expand = function (project) {
                if (project.showDetail) {
                    project.showDetail = false;
                    project.expendClass = "fa-chevron-circle-down";
                } else {
                    angular.forEach($scope.list, function (p, i) {
                        var showDetail = p === project;
                        p.showDetail = showDetail;
                        p.expendClass = showDetail ? "fa-chevron-circle-up" : "fa-chevron-circle-down";
                    });
                }
            };
            $scope.showAdvanceSearch = function () {
                $modal.open({
                    size: "lg",
                    backdrop: 'static',
                    resolve: {
                        condition: function () {
                            return angular.copy($scope.searchCondition);
                        }
                    },
                    templateUrl: Utils.ServiceURI.WebAddress() + "Template/ProjectAdvanceSearch",
                    controller: ["$scope", "$modalInstance", "condition", function ($scope, $modalInstance, condition) {
                        $scope.condition = condition;

                        $scope.seachOrg = function (level, name) {
                            return $http.get(Utils.ServiceURI.ApiDelegate, {
                                cache: false,
                                params: {
                                    url: Utils.ServiceURI.FrameAddress() + "api/org/5",
                                    type: level,
                                    name: name,
                                    status: 1
                                }
                            }).then(function (response) {
                                return $.map(response.data, function (d, i) {
                                    return d.NameZHCN;
                                });
                            });
                        };

                        $scope.searchEmployee = function (name) {
                            return $http.get(Utils.ServiceURI.ApiDelegate, {
                                cache: false,
                                params: {
                                    url: Utils.ServiceURI.FrameAddress() + "api/user/5",
                                    name: name
                                }
                            }).then(function (response) {
                                return response.data;
                            });
                        };

                        $scope.selectStore = function () {
                            $selectStore.open({
                                selectedStores: !!$scope.condition.USCode ?
                                    $.map($scope.condition.USCode.split(","), function (c, i) {
                                        return {
                                            Code: c
                                        };
                                    }) : [],
                                userCode: window["currentUser"] ? window["currentUser"].Code : null,
                                OnStoreSelected: function (stores) {
                                    $scope.condition.USCode = $.map(stores, function (s, i) {
                                        return s.Code;
                                    }).join(",");
                                }
                            });
                        };

                        $scope.searchStore = function (name) {
                            return $http.get(Utils.ServiceURI.ApiDelegate, {
                                cache: false,
                                params: {
                                    url: Utils.ServiceURI.FrameAddress() + "api/store/fuzzy/5",
                                    name: name
                                }
                            }).then(function (response) {
                                return response.data;
                            });
                        };

                        $scope.openDatepicker = function ($event, dateOpenTag) {
                            $event.preventDefault();
                            $event.stopPropagation();
                            $scope.condition[dateOpenTag] = true;
                        };

                        $scope.cancel = function () {
                            $modalInstance.dismiss("cancel");
                        };

                        $scope.ok = function () {
                            $modalInstance.close($scope.condition);
                        };
                    }]
                }).result.then(function (condition) {
                    $scope.searchCondition = angular.copy(condition);
                    $scope.search();
                });
            }

            $scope.outputExcel = function () {
                projectService.outputExcel($scope.searchCondition).$promise.then(function (data) {
                    window.open(Utils.ServiceURI.Address() + "Temp\\" + data.fileName);
                });
            };

            $scope.holdingStatusChange = function (pkgHolding) {
                if (pkgHolding.Status) {
                    var confirmMsg, successMsg, errorMsg;
                    if (pkgHolding.Status == 1) {
                        confirmMsg = "[[[确认允许该餐厅进入Reimage PKG流程吗？]]]";
                        successMsg = '呈递成功';
                        errorMsg = '呈递失败';
                    } else {
                        confirmMsg = "[[[确认需要撤销呈递吗？]]]";
                        successMsg = '[[[撤销成功]]]';
                        errorMsg = '[[[撤销失败]]]';
                    }

                    messager.confirm(confirmMsg, "fa-warning c_orange")
                        .then(function (result) {
                            if (result) {
                                projectService.changePackageHoldingStatus(pkgHolding).$promise.then(function (response) {
                                    messager.showMessage(successMsg, "fa-check c_green");
                                }, function (error) {
                                    pkgHolding.Status = 2;
                                    messager.showMessage(errorMsg + error.data.Message, "fa-warning c_orange");
                                });
                            }
                        });
                }
            }
        }])
        .controller("projectDetailCtrl", [
            "$scope",
            "$http",
            "$routeParams",
            "taskWorkService",
            "projectUsersService",
            "flowService",
            "approveDialogService",
            function ($scope, $http, $routeParams, taskWorkService, projectUsersService, flowService, approveDialogService) {
                $scope.projectId = $routeParams.projectId;
                $scope.flowCode = $routeParams.flowCode;
                $scope.linkFromList = $routeParams.linkFromList;
                projectUsersService.queryViewers($scope.projectId).then(function (response) {
                    $scope.viewers = response.data;
                    $scope.viewerLoaded = true;
                });
            }
        ])
        .directive("projectNodeInfo", [
            "$modal",
            "messager",
            "flowService",
            "projectService",
            "taskWorkService",
            "approveDialogService",
            function ($modal, messager, flowService, projectService, taskWorkService, approveDialogService) {
                return {
                    restrict: "EA",
                    replace: true,
                    scope: {
                        projectId: "@",
                        flowCode: "@",
                        visible: "=",
                        usCode: "@"
                    },
                    templateUrl: Utils.ServiceURI.AppUri + "Module/ProjectNodeInfo",
                    link: function ($scope, element, attrs) {
                        $scope.loadNodes = function () {
                            $scope.loaded = false;
                            flowService.getNodes({
                                projectId: $scope.projectId,
                                flowCode: $scope.flowCode
                            }).$promise.then(function (nodes) {
                                angular.forEach(nodes, function (n, i) {
                                    n.operatorHTML = "<img src='" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' />";
                                });
                                $scope.nodes = nodes;
                                $scope.loaded = true;
                            });
                        };
                        $scope.$watch("visible", function (val) {
                            if (val === true) {
                                $scope.loadNodes();
                            }
                        });
                        $scope.loadOperators = function (node) {
                            taskWorkService.getOperators(node.Code, $scope.projectId).then(function (response) {
                                var operators = response.data;
                                var OperatorHTML = "";
                                if (!!operators && operators.length > 0) {
                                    OperatorHTML += "<ul class='node-operators'>";
                                    angular.forEach(operators, function (o, i) {
                                        OperatorHTML += "<li>" + o.OperateMsgZHCN + "</li>";
                                    });
                                    OperatorHTML += "</ul>";
                                } else if (node.ProgressRate === 100) {
                                    OperatorHTML = "<p class='node-operators-none'>[[[已完成]]]</p>";
                                } else {
                                    OperatorHTML = "<p class='node-operators-none'>[[[暂无处理人]]]</p>";
                                }
                                node.operatorHTML = OperatorHTML;
                            });
                        };
                        $scope.editApprover = function (node) {
                            approveDialogService.open($scope.projectId, node.Code, 'ProjectList', $scope.usCode).then(function (projectDto) {
                                if (!projectDto.ProjectId) {
                                    var approveUsers = projectDto;
                                    //Closure
                                    if (projectDto.selMarketMgr) {
                                        approveUsers.MarketMgr = projectDto.selMarketMgr;
                                        approveUsers.RegionalMgr = projectDto.selRegionalMgr;
                                        approveUsers.MDD = projectDto.selDD;
                                        approveUsers.DD = projectDto.selDD;
                                        approveUsers.GM = projectDto.selGM;
                                        approveUsers.FC = projectDto.selFC;
                                        approveUsers.VPGM = projectDto.selVPGM;
                                        approveUsers.CDO = projectDto.selCDO;
                                        approveUsers.CFO = projectDto.selCFO;
                                        approveUsers.ManagingDirector = projectDto.selMngDirector;
                                    }
                                    projectDto = {
                                        ProjectId: $scope.projectId,
                                        FlowCode: node.Code,
                                        ApproveUsers: approveUsers
                                    };
                                }
                                projectService.changeWorkflowApprovers(projectDto).$promise.then(function () {
                                    messager.showMessage("[[[操作成功]]]", "fa-check c_green");
                                    $scope.loadOperators(node);
                                }, function () {
                                    messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                                });
                            });
                        };
                        $scope.recall = function (node) {
                            messager.confirm("[[[确认需要撤销吗？]]]", "fa-warning c_orange")
                            .then(function (result) {
                                if (result) {
                                    $modal.open({
                                        templateUrl: Utils.ServiceURI.AppUri + "Template/Recall",
                                        backdrop: 'static',
                                        size: "lg",
                                        resolve: {
                                            entity: function () {
                                                return {
                                                    Comment: ""
                                                };
                                            }
                                        },
                                        controller: ["$scope", "$modalInstance", "entity", function ($modalScope, $modalInstance, entity) {
                                            $modalScope.entity = entity;
                                            $modalScope.ok = function () {
                                                $modalInstance.close($modalScope.entity);
                                            };

                                            $modalScope.cancel = function () {
                                                $modalInstance.dismiss("");
                                            };
                                        }]
                                    }).result.then(function (entity) {
                                        projectService.recall({
                                            projectId: $scope.projectId,
                                            flowCode: node.Code,
                                            projectComment: entity.Comment
                                        }).$promise.then(function () {
                                            messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                                                $scope.loadNodes();
                                            });
                                        }, function () {
                                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                                        });
                                    });
                                }
                            });
                        };
                        $scope.edit = function (node) {
                            switch (node.Code) {
                                case "Closure_ClosurePackage":
                                case "MajorLease_Package":
                                case "TempClosure_ClosurePackage": {
                                    messager.confirm("你是否要重新报Package？一旦重报系统将重新审批流程。", "fa-warning c_orange")
                                    .then(function (result) {
                                        if (result) {
                                            $modal.open({
                                                backdrop: "static",
                                                size: "sm",
                                                resolve: {
                                                    nodes: function () { return angular.copy($scope.nodes); }
                                                },
                                                templateUrl: Utils.ServiceURI.AppUri + "Module/EditProjectPackage",
                                                controller: [
                                                    "$scope",
                                                    "$modalInstance",
                                                    "nodes",
                                                    function ($modalScope, $modalInstance, nodes) {
                                                        $modalScope.canEditNodes = $.grep(nodes || [], function (n, i) {
                                                            if (n.ExecuteSequence == node.ExecuteSequence) {
                                                                n.canEdit = true;
                                                            }
                                                            return n.ExecuteSequence <= node.ExecuteSequence;
                                                        }).sort(function (n1, n2) {
                                                            return n2.ExecuteSequence - n1.ExecuteSequence;
                                                        });
                                                        $modalScope.editChanged = function (_node) {
                                                            angular.forEach($modalScope.canEditNodes || [], function (n, i) {
                                                                if (n.ExecuteSequence < _node.ExecuteSequence) {
                                                                    if (_node.edit) {
                                                                        n.canEdit = n.ExecuteSequence == _node.ExecuteSequence - 1;
                                                                    } else {
                                                                        n.canEdit = false;
                                                                        n.edit = false;
                                                                    }
                                                                };
                                                            });
                                                        };
                                                        $modalScope.ok = function () {
                                                            var editNodes = $.grep($modalScope.canEditNodes || [], function (n, i) {
                                                                return n.edit === true;
                                                            });

                                                            if (editNodes.length == 0) {
                                                                messager.showMessage("请选择要Edit的项目", "fa-warning c_orange");
                                                                return;
                                                            }
                                                            $modalInstance.close(editNodes);
                                                        };
                                                        $modalScope.cancel = function () {
                                                            $modalInstance.dismiss('');
                                                        }
                                                    }
                                                ]
                                            }).result.then(function (editNodes) {
                                                projectService.editMulty({
                                                    ProjectId: $scope.projectId,
                                                    EditProjects: editNodes
                                                }).$promise.then(function (editResult) {
                                                    messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                                                        $scope.loadNodes();
                                                    });
                                                }, function () {
                                                    messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                                                });
                                            });
                                        }
                                    });

                                }
                                    break;
                                default: {
                                    messager.confirm("[[[审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                                    .then(function (result) {
                                        if (result) {
                                            projectService.edit({
                                                projectId: $scope.projectId,
                                                flowCode: node.Code
                                            }).$promise.then(function (editResult) {
                                                messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                                                    //$scope.loadNodes();
                                                    $scope.$parent.search();
                                                });
                                            }, function () {
                                                messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                                            });
                                        }
                                    });
                                }
                                    break;
                            };


                        };
                    }
                };
            }
        ])
        .directive("projectDetailInfo", [
            "$http",
            "projectUsersService",
            "projectService",
            "messager",
        function ($http, projectUsersService, projectService, messager) {
            return {
                restrict: "EA",
                replace: true,
                scope: {
                    projectId: "@",
                    usCode: "=",
                    detail: "=?",
                    store: "=?"
                },
                template: "<div ng-include='projectTplUrl'></div>",
                link: function ($scope, element, attrs) {
                    var templateUrl = Utils.ServiceURI.AppUri, projectType = /[a-z]+/ig.exec($scope.projectId)[0].toLowerCase();
                    var api = Utils.ServiceURI.Address();
                    var saveApi = Utils.ServiceURI.Address();
                    $scope.innerEditing = false;
                    $scope.enableEdit = true;
                    projectUsersService.canInnerEdit($scope.projectId).then(function (response) {
                        $scope.canInnerEdit = response.data;
                    });
                    switch (projectType) {
                        case "closure":
                            $scope.projectTplUrl = Utils.ServiceURI.AppUri + "Closure/ProjectDetail";
                            api += "api/closure/project/" + $scope.projectId;
                            saveApi += "api/Closure/UpdateClosureInfo";
                            break;
                        case "majorlease":
                            $scope.projectTplUrl = Utils.ServiceURI.AppUri + "MajorLease/ProjectDetail";
                            api += "api/MajorLease/GetMajorInfo?projectId=" + $scope.projectId;
                            break;
                        case "tpcls":
                            $scope.projectTplUrl = Utils.ServiceURI.AppUri + "TempClosure/ProjectDetail";
                            api += "api/tempClosure/get?projectId=" + $scope.projectId;
                            saveApi += "api/tempClosure/update";
                            break;
                        case "reimage":
                            $scope.projectTplUrl = Utils.ServiceURI.AppUri + "Reimage/ProjectDetail";
                            api += "api/reimage/project/" + $scope.projectId;
                            saveApi += "api/reimage/update";
                            break;
                        case "renewal":
                            {
                                $scope.projectTplUrl = Utils.ServiceURI.AppUri + "Renewal/ProjectDetail";
                                api += "api/renewal/get?projectId=" + $scope.projectId;
                                saveApi += "api/renewal/update";
                                $scope.$watch("detail.NewLeaseStartDate+detail.NewLeaseEndDate", function () {
                                    if (!!$scope.detail) {
                                        if (!!$scope.detail.NewLeaseStartDate && !!$scope.detail.NewLeaseEndDate) {
                                            $scope.detail.RenewalYears = Math.round(Utils.caculator.multiply(moment($scope.detail.NewLeaseEndDate).diff($scope.detail.NewLeaseStartDate) / 31536000000, 10)) / 10;
                                        } else {
                                            $scope.detail.RenewalYears = null;
                                        }
                                    }
                                });
                            }
                            break;
                        case "rebuild":
                            $scope.projectTplUrl = Utils.ServiceURI.AppUri + "Rebuild/ProjectDetail";
                            api += "api/Rebuild/GetRebuildInfo?projectId=" + $scope.projectId;
                            saveApi += "api/Rebuild/Update";
                            break;
                    }
                    var load = function () {
                        $http.get(api, {
                            cache: false
                        }).success(function (detail) {
                            $scope.detail = detail;
                            $scope.usCode = detail.USCode;
                            if (projectType == "tpcls") {
                                $http.get(Utils.ServiceURI.Address() + "api/tempClosure/enableEditProject?projectId=" + $scope.projectId, {
                                    cache: false
                                }).success(function (detail) {
                                    $scope.enableEdit = !detail;
                                });
                            }
                        });
                    };
                    load();
                    $scope.edit = function () {
                        $scope.innerEditing = true;
                    };
                    $scope.save = function () {
                        $http.post(saveApi, $scope.detail).success(function () {
                            messager.showMessage("[[[保存成功]]]", "fa_check c_green");
                            $scope.innerEditing = false;
                            load();
                        });
                    };
                    $scope.openDate = function ($event, tag) {
                        $event.preventDefault();
                        $event.stopPropagation();
                        $scope[tag] = true;
                    };
                }
            };
        }]);
    function projectSearchCondition() {
        this.ProjectId = "";
        this.CreateDate = null;
        this.CreateDateFrom = null;
        this.CreateDateTo = null;
        this._IsOpenCreateDate = false;
        this._IsOpenCreateDateFrom = false;
        this._IsOpenCreateDateTo = false;
        this.StoreType = "";
        this.ProjectType = "";
        this.PortfolioType = "";
        this.Status = "";
        this.StoreStatus = "";
        this.OpenDate = null;
        this.ReOpenDateFrom = null;
        this.ReOpenDateTo = null;
        this._IsOpenOpenDate = false;
        this._IsOpenReOpenDateFrom = false;
        this._IsOpenReOpenDateTo = false;
        this.GBDateFrom = null;
        this.GBDateTo = null;
        this._IsOpenGBDateFrom = false;
        this._IsOpenGBDateTo = false;
        this.CFDateFrom = null;
        this.CFDateTo = null;
        this._IsOpenCFDateFrom = false;
        this._IsOpenCFDateTo = false;
        this.Actor = "";
        this.AssetRep = "";
        this.AssetManager = "";
        this.Finance = "";
        this.PM = "";
        this.CloseDate = null;
        this._IsOpenCloseDate = false;
        this.Legal = "";
        this.USCode = "";
        this.ReImagingDate = null;
        this.ReImagingDateFrom = null;
        this.ReImagingDateTo = null;
        this._IsOpenReImagingDate = false;
        this._IsOpenReImagingDateFrom = null;
        this._IsOpenReImagingDateTo = null;
        this.StoreName = "";
        this.DesignType = ""
        this.Province = "";
        this.City = "";
        this.HoldingStatus = null;
    }
}();