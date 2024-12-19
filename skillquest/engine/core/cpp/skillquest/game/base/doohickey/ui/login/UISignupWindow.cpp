/**
 * @author omnisudo
 * @date 2024.08.01
 */

#include "skillquest/game/base/doohickey/ui/login/UISignupWindow.hpp"
#include "skillquest/game/base/doohickey/ui/login/UILoginWindow.hpp"
#include "skillquest/sh.api.hpp"
#include "imgui.h"
#include "imgui_stdlib.h"
#include "skillquest/crypto.sha256.hpp"

namespace skillquest::game::base::doohickey::ui::login {
	UISignupWindow::UISignupWindow ( const CreateInfo& info ) : UICredentialsWindow{ info } {
	
	}
	
	void UISignupWindow::draw () {
		ImGui::Begin( "Signup" );

        ImGui::InputText( "server", &address() );
		ImGui::InputText( "email", &email() );
		ImGui::InputText( "password", &password(), ImGuiInputTextFlags_Password );
		ImGui::InputText( "confirm password", &_confirm, ImGuiInputTextFlags_Password );
		
		if ( ImGui::Button( "Confirm" ) && password() == _confirm ) {
			if ( !connection() || !connection()->connected() ) {
				std::thread(
						[ this ] () -> void {
							try {
								auto hashed = crypto::hash::SHA256::string( password() );

								connection( sq::shared()->network()->clients()
													.connect( { address(), 3698 }, email(), hashed, true ) );
								
								connection()->wait_for( std::chrono::seconds{ 5 } );
								if ( connection()->connected() && connection()->authenticated() ) {
									connected();
								} else {
									sq::shared()->logger()->trace(
											"User {0} failed to authenticate",
											connection()->email()
									);
								}
							} catch ( const std::exception& e ) {
								sq::shared()->logger()->error( "Failed to login: {0}", e.what() );
							}
						}
				).detach();
			}
		}
		ImGui::SameLine();
		if ( ImGui::Button( "Back" ) ) {
			changeToLogin();
		}
		ImGui::End();
	}
	
	void UISignupWindow::changeToLogin () {
		stuff().create< UILoginWindow >(
				{
						.doohickey = {
								.uri = { "ui://skill.quest/window/login" }
						},
						.email = this->email(),
						.password = this->password(),
						.address = this->address()
				}
		);
		stuff().remove( stuff::Thing::self() );
	}
}
