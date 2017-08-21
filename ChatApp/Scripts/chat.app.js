
var chat = $.connection.chatHub;

chat.client.refreshOnlineUsers = function () {
    refreshOnlineUsers();
};
chat.client.receiveNotification = function (notificationType, userInfo, notificationID, notificationCounts) {
    changeUserNotificationCounts(notificationCounts);
    var notificationMessage = '';
    if (notificationType == "FriendRequest") {
        notificationMessage = '<span><a href="/User/Profile/' + userInfo.UserID + '"><img src="' + userInfo.ProfilePicture + '" class="profilePictureCircle" />&nbsp;&nbsp;&nbsp;' + userInfo.Name + '</a><br /><span class="pull-right"><span style="font-size:11px;">Friend Request</span> &nbsp;&nbsp;<input type="button" class="btn btn-success btn-xs request-response" data-user-id="' + userInfo.UserID + '" data-value="Accepted" value="Accept" />&nbsp;&nbsp;<input type="button" class="btn btn-danger btn-xs request-response" data-user-id="' + userInfo.UserID + '" data-value="Rejected" value="Reject" /></span><br /></span>';
    }
    else if (notificationType == "FriendRequestAccepted") {
        notificationMessage = '<span><a href="/User/Profile/' + userInfo.UserID + '"><img src="' + userInfo.ProfilePicture + '" class="profilePictureCircle" />&nbsp;&nbsp;&nbsp;' + userInfo.Name + '</a><br /><span class="pull-right"><span style="font-size:11px;">Accepted your request</span></span><br /></span>';
    }
    var notificationHtml = '<div data-notificationID="' + notificationID + '" style="display:none" id="divNotificationPopUp-' + notificationID + '" class="alert alert-dismissible alert-info divNotificationPopup"><button type="button" class="close btnCloseNotification" data-notificationID="' + notificationID + '">&times;</button>' + notificationMessage + '</div>';
    $('.new-notificaion-window').append(notificationHtml);
    $(document).find('#divNotificationPopUp-' + notificationID + '').animate({ "opacity": "show", top: "100" }, 500);
    setTimeout(function () {
        removeNotificationPop(notificationID);
    }, 60000)
}
chat.client.refreshNotificationCounts = function (notificationCounts) {
    changeUserNotificationCounts(notificationCounts)
}
chat.client.removeNotification = function (notificationID) {
    removeNotificationPop(notificationID);
    var notificationRow = $(document).find('div[class$="divNotification"][data-notificationid="' + notificationID + '"]');
    removeHtmlElement(notificationRow);
}
chat.client.addNewChatMessage = function (messageRow, fromUserId, toUserId, fromUserName, fromUserProfilePic, toUserName, toUserProfilePic) {
    var currentUserId = $('#hdfLoggedInUserID').val();
    var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
    if (currentChatUserID == fromUserId || currentChatUserID == toUserId) {
        createNewMessageBlock(fromUserName, fromUserProfilePic, messageRow.CreatedOn, messageRow.Message, (currentUserId == fromUserId ? 'right' : 'left'), messageRow.ChatMessageID, messageRow.Status);
        if (currentUserId != fromUserId) {
            var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
            setTimeout(function () {
                UpdateChatMessageStatus(messageRow.ChatMessageID, currentChatUserID);
            }, 100);
        }
    }
    if (currentUserId != fromUserId) {
        var windowActive = $('#hdfWindowIsActiveOrNot').val();
        if (windowActive == 'False') {
            document.title = "Message received from " + fromUserName;
        }
    }
    addChatMessageCount(currentUserId, fromUserId, fromUserName, fromUserProfilePic, toUserId, toUserName, toUserProfilePic)
}
chat.client.userIsTyping = function (fromUserId) {
    var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
    if (currentChatUserID == fromUserId) {
        $(document).find('div.chat-user-status').html('typing...');
        setTimeout(function () {
            $(document).find('div.chat-user-status').html('');
        }, 1000);
    }
}
chat.client.updateMessageStatusInChatWindow = function (messageID, currentUserID, fromUserID) {
    if (messageID > 0) {
        var message = $(document).find('span[class="chat-message-status"][data-chat-message-id="' + messageID + '"]');
        if (message.length > 0) {
            $(message).text('Viewed');
        }
    }
    else {
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        if (currentChatUserID == currentUserID) {
            var messages = $(document).find('span[class="chat-message-status"]');
            $(messages).each(function (index, item) {
                $(item).text('Viewed');
            });
        }
    }
}
chat.client.refreshOnlineUserByUserID = function (userID, isOnline, lastSeen) {
    var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
    if (currentChatUserID == userID) {
        if (isOnline == true) {
            $(document).find('span[class="spn-chat-user-online-status"]').html('<i class="fa fa-circle online-circle chat-user-online-status"></i>Online');
        }
        else {
            $(document).find('span[class="spn-chat-user-online-status"]').text('Last seen : ' + lastSeen + '');
        }
    }
}
$.connection.transports.longPolling.supportsKeepAlive = function () {
    return false;
}
$.connection.hub.qs = "UserID=" + $('#hdfLoggedInUserID').val();
//$.connection.hub.start({ transport: ['longPolling', 'webSockets'], waitForPageLoad: false }).done(function () {
$.connection.hub.start().done(function () {
    refreshUserNotificationCounts($('#hdfLoggedInUserID').val());
    refreshOnlineUsers();
    refreshRecentChats();
});
$.connection.hub.disconnected(function () {
    setTimeout(function () {
        $.connection.hub.start();
    }, 5000); // Restart connection after 5 seconds.
});
$(document).ready(function () {
    $('#hdfWindowIsActiveOrNot').val('True');
    $(window).blur(function () {
        $('#hdfWindowIsActiveOrNot').val('False');
    });
    $(window).focus(function () {
        $('#hdfWindowIsActiveOrNot').val('True');
        document.title = "Chat";
    });
  
});
function sendResponseToRequest(userid, requestResponse, loggedInUserID) {
    chat.server.sendResponseToRequest(userid, requestResponse, loggedInUserID);
}
function sendFriendRequest(userID, loggedInUserID) {
    chat.server.sendRequest(userID, loggedInUserID);
}
function refreshUserNotificationCounts(loggedInUserID) {
    chat.server.refreshNotificationCounts(loggedInUserID);
}
function changeUserNotificationStatus(notificationID) {
    chat.server.changeNotitficationStatus(notificationID, $('#hdfLoggedInUserID').val());
}
function refreshOnlineUsers() {
    $(document).find('.online-friends').load('/User/_OnlineFriends', function () {
        var recentChats = $(document).find('.recent-chats').find('a');
        $(recentChats).each(function (cIndex, cItem) {
            changeUserOnlineStatus(cItem);
        });
        var friends = $('.user-friends');
        if (friends.length > 0) {
            var friendList = $(friends).find('li');
            console.log(friendList);
            $(friendList).each(function (cIndex, cItem) {
                changeUserOnlineStatus(cItem);
            });
        }
    });
}
function changeUserOnlineStatus(cItem) {
    $(cItem).find('img').removeClass('online-user-profile-pic');
    var userID = $(cItem).attr('data-user-id');
    var onlineItem = $(document).find('.online-friends').find('a[data-user-id="' + userID + '"]');
    if (onlineItem.length > 0) {
        $(cItem).find('img').addClass('online-user-profile-pic');
    }
}
function changeUserNotificationCounts(notificationCounts) {
    if (notificationCounts != null && notificationCounts != '' && notificationCounts != 0 && notificationCounts != '0') {
        $('.user-notification-count').text(notificationCounts);
    }
    else {
        $('.user-notification-count').text('');
    }
}
function removeHtmlElement(ele) {
    if (ele.length > 0) {
        ele.animate({ "opacity": "hide", top: "100" }, 500);
        setTimeout(function () {
            ele.remove();
        }, 500);
    }
}
function removeNotificationPop(notificationID) {
    var notificationPopup = $(document).find('#divNotificationPopUp-' + notificationID + '');
    removeHtmlElement(notificationPopup);
}
function unfriendUser(friendMappingID) {
    chat.server.unfriendUser(friendMappingID);
}
function initiateChat(toUserID) {
    $("#divBody").html('');
    $("#divBody").load('/Chat/_Messages/' + toUserID, function () {
        $("#divBody").animate({ "opacity": "show", top: "100" }, 500);
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        UpdateChatMessageStatus(0, currentChatUserID);
        var recentChat = $(document).find('.recent-chats').find('a[data-user-id="' + toUserID + '"]');
        if (recentChat.length > 0) {
            var badge = $(recentChat).find('span');
            if ($(badge).hasClass('chat-message-count') && !$(badge).hasClass('hide')) {
                $(badge).text('');
                $(badge).addClass('hide');
            }
        }
    });
}
function createNewMessageBlockHtml(name, profilePicture, createOn, message, align, chatMessageID, status) {
    var html = '<li class="' + align + '" data-chat-message-id="' + chatMessageID + '"><img src="' + profilePicture + '" alt="' + name + '" class="avatar"><span class="message"><span class="arrow"></span><span class="from">' + name + '</span>&nbsp;<span class="time">' + createOn + '</span>' + (align == 'right' ? '<span data-chat-message-id="' + chatMessageID + '" class="chat-message-status">' + status + '</span>' : '') + '<br /><span class="text">' + message + '</span></span></li>';
    return html;
}
function sendChatMessage() {
    var fromUserID = $('#hdfLoggedInUserID').val();
    var fromUserName = $('#hdfLoggedInUserName').val();
    var fromUserPrifilePic = $('#hdfLoggedInUserProfilePicture').val();
    var chatMessage = $(document).find('.txt-chat-message').val();
    var toUserID = $(document).find('.hdf-current-chat-user-id').val();
    var toUserName = $(document).find('.hdf-current-chat-user-name').val();
    var toUserProfilePic = $(document).find('.hdf-current-chat-user-profile-picture').val();
    if (chatMessage != null && chatMessage != '') {
        chat.server.sendMessage(fromUserID, toUserID, chatMessage, fromUserName, fromUserPrifilePic, toUserName, toUserProfilePic);
        $(document).find('.txt-chat-message').val('');
    }
}
function createNewMessageBlock(name, profilePicture, createOn, message, align, chatMessageID, status) {
    $(document).find('ul.chat').append(createNewMessageBlockHtml(name, profilePicture, createOn, message, align, chatMessageID, status));
    $(document).find("div.right-chat-panel").animate({ scrollTop: $(document).find("div.right-chat-panel")[0].scrollHeight }, 500);
}
function sendUserTypingStatus() {
    var toUserID = $(document).find('.hdf-current-chat-user-id').val();
    var fromUserID = $('#hdfLoggedInUserID').val();
    chat.server.sendUserTypingStatus(toUserID, fromUserID);
}
function refreshRecentChats() {
    $('.recent-chats').load('/User/_RecentChats', function () {

    });
}
function addChatMessageCount(currentUserId, fromUserId, fromUserName, fromUserProfilePic, toUserId, toUserName, toUserProfilePic) {
    var recentChatWindow = $(document).find('.recent-chats');
    var recentChatItem = $(recentChatWindow).find('a[data-user-id="' + ((currentUserId != fromUserId) ? fromUserId : toUserId) + '"]');
    if (recentChatItem.length > 0) {
        $(recentChatItem).parent().prepend(recentChatItem);
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        if (currentUserId != fromUserId && (currentChatUserID != fromUserId)) {
            var messageCountItem = $(recentChatItem).find('span[data-user-id="' + fromUserId + '"]');
            var count = messageCountItem.text();
            if (count.match(/^\d+$/)) {
                $(messageCountItem).text(parseInt(count) + 1);
            }
            else {
                $(messageCountItem).removeClass('hide');
                $(messageCountItem).text(1);
            }
        }
    }
    else {
        var html = '';
        if (currentUserId != fromUserId) {
            var uName = fromUserName.split(' ');
            html = '<a href="javascript:;" data-user-id="' + fromUserId + '" class="list-group-item chat-user"><img src="' + fromUserProfilePic + '" class="profilePictureCircle online-user-profile-pic" />&nbsp;&nbsp;&nbsp;' + uName[0] + '<span class="custom-badge chat-message-count" data-user-id="' + fromUserId + '">1</span></a>';
        }
        else {
            var uName = toUserName.split(' ');
            html = '<a href="javascript:;" data-user-id="' + toUserId + '" class="list-group-item chat-user"><img src="' + toUserProfilePic + '" class="profilePictureCircle online-user-profile-pic" />&nbsp;&nbsp;&nbsp;' + uName[0] + '<span class="custom-badge chat-message-count hide" data-user-id="' + toUserId + '"></span></a>';
        }
        if ($('.no-recent-chats').length > 0) {
            $('.no-recent-chats').remove();
        }
        $(recentChatWindow).prepend(html);
    }
}
function UpdateChatMessageStatus(messageID, fromUserID) {
    var currentUserID = $('#hdfLoggedInUserID').val();
    chat.server.updateMessageStatus(messageID, currentUserID, fromUserID);
}
function GetOldMessages() {
    var isOldMessageExsit = $(document).find('.hdf-old-messages-exist');
    if ($(isOldMessageExsit).val() == "True") {
        var currentChatUserID = $(document).find('.hdf-current-chat-user-id').val();
        var lastMessageID = $(document).find('.hdf-last-chat-message-id').val();
        console.log($(document).find("div.right-chat-panel").scrollTop())
        $.get('/Chat/GetRecentMessages?Id=' + currentChatUserID + '&lastChatMessageId=' + lastMessageID, function (messages) {
            if (messages.ChatMessages.length > 0) {
                $(isOldMessageExsit).val((messages.ChatMessages.length < 20 ? "False" : "True"));
                $(document).find('.hdf-last-chat-message-id').val(messages.LastChatMessageId);
                var html = '';
                var currentUserId = $('#hdfLoggedInUserID').val();
                var fromUserName = $('#hdfLoggedInUserName').val();
                var fromUserPrifilePic = $('#hdfLoggedInUserProfilePicture').val();
                var chatMessage = $(document).find('.txt-chat-message').val();
                var toUserID = $(document).find('.hdf-current-chat-user-id').val();
                var toUserName = $(document).find('.hdf-current-chat-user-name').val();
                var toUserProfilePic = $(document).find('.hdf-current-chat-user-profile-picture').val();
                $(messages.ChatMessages).each(function (index, item) {
                    if (item.FromUserID == currentUserId) {
                        html += createNewMessageBlockHtml(fromUserName, fromUserPrifilePic, item.CreatedOn, item.Message, "right", item.ChatMessageID, item.Status);
                    }
                    else {
                        html += createNewMessageBlockHtml(toUserName, toUserProfilePic, item.CreatedOn, item.Message, "left", item.ChatMessageID, item.Status);
                    }
                });
                var firstMsg = $('ul.chat li:first');
                $(document).find('ul.chat').prepend(html);
                $(document).find("div.right-chat-panel").scrollTop(firstMsg.offset().top);
            }
            else {
                $(isOldMessageExsit).val("False");
            }
        });
    }
}
